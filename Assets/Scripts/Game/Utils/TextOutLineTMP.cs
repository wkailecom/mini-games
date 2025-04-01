using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShineEngine
{
    public class TextOutLineTMP : UIBehaviour, IMaterialModifier
    {
        [Header("粗细度")]
        [SerializeField]
        public bool _useFace = false;

        [SerializeField]
        Color _faceColor = Color.white;

        [SerializeField, Range(0f, 1f)] float _faceSoftness = 0f;
        [SerializeField, Range(-1f, 1f)] float _faceDilate = 0f;

        
        [Header("描边")]
        [SerializeField] 
        public bool _useOutline = false;

        [SerializeField]
        public Color _outlineColor = Color.black;

        [SerializeField, Range(0f, 1f)]
        public float _outlineWidth = 0f;

        
        [Header("阴影")][SerializeField]
        public bool _useShadow = false;

        [SerializeField]
        public  Color _shadowColor = Color.white;

        [SerializeField, Range(-1f, 1f)]
        public float _shadowOffectX = 0f;
        [SerializeField, Range(-1f, 1f)]
        public float _shadowOffectY = 0f;
        
        [SerializeField, Range(0, 1f)]
        public float _shadowSoftness = 0f;
        [SerializeField, Range(-1f, 1f)]
        public float _shadowDilate = 0f;
     
        
        Graphic? _graphic;
        TextMeshProUGUI? _textComponent;
        Material? _material;

        Graphic? graphic => _graphic ??= GetComponent<Graphic>();

        TextMeshProUGUI? textComponent => _textComponent ??= GetComponent<TextMeshProUGUI>();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }

            if (textComponent != null && textComponent.font != null)
            {
                textComponent.fontMaterial = textComponent.font.material;
            }
        }

        protected override void OnDestroy()
        {
            DestroyMaterial();
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!IsActive() || graphic == null || textComponent == null)
            {
                return baseMaterial;
            }

            if (_material != null && _material.name != baseMaterial.name)
            {
                DestroyMaterial();
            }

            var isNewMaterial = false;
            if (_material == null)
            {
                _material = new Material(baseMaterial)
                {
                    name = baseMaterial.name + "(TMProUguiOutline Instance)",
                    hideFlags = HideFlags.HideAndDontSave,
                    shaderKeywords = baseMaterial.shaderKeywords
                };

                isNewMaterial = true;
            }

            _material.CopyPropertiesFromMaterial(textComponent.font.material);

            if (_useFace && _material.HasProperty(ShaderUtilities.ID_FaceColor))
            {
                _material.SetColor(ShaderUtilities.ID_FaceColor, _faceColor);
                _material.SetFloat(ShaderUtilities.ID_FaceDilate, _faceDilate);
                _material.SetFloat(ShaderUtilities.ID_OutlineSoftness, _faceSoftness);
            }

            if (_useOutline && _material.HasProperty(ShaderUtilities.ID_OutlineColor))
            {
                _material.EnableKeyword(ShaderUtilities.Keyword_Outline);
                _material.SetColor(ShaderUtilities.ID_OutlineColor, _outlineColor);
                _material.SetFloat(ShaderUtilities.ID_OutlineWidth, _outlineWidth);
            }
            else
            {
                _material.DisableKeyword(ShaderUtilities.Keyword_Outline);
            }
            
            //阴影
            if (_useShadow && _material.HasProperty(ShaderUtilities.ID_UnderlayColor))
            {
                _material.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                _material.SetColor(ShaderUtilities.ID_UnderlayColor, _shadowColor);
                _material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, _shadowOffectX);
                _material.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, _shadowOffectY);
                _material.SetFloat(ShaderUtilities.ID_UnderlayDilate,_shadowDilate);
                _material.SetFloat(ShaderUtilities.ID_UnderlaySoftness, _shadowSoftness);
            }
            else
            {
                _material.DisableKeyword(ShaderUtilities.Keyword_Underlay);
            }

            if (_material.HasProperty(ShaderUtilities.ID_StencilComp))
            {
                var stencilID = baseMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                var stencilComp = baseMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                _material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                _material.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);
            }

            if (isNewMaterial)
            {
                textComponent.fontMaterial = _material;
                textComponent.ForceMeshUpdate();
            }
            else
            {
                textComponent.UpdateMeshPadding();
            }

            return _material;
        }

        void DestroyMaterial()
        {
            if (_material != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(_material);
#else
            Destroy(_material);
#endif
                _material = null;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (!IsActive() || graphic == null) return;
            graphic.SetMaterialDirty();
        }
#endif
    }
}