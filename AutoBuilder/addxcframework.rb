#!/usr/bin/env ruby
# -*- coding: UTF-8 -*-

require 'xcodeproj'

# 确保提供了项目目录
if ARGV.empty?
  puts "无效: #{$0} <project_directory>"
  exit 1
end
# 定义项目目录路径
project_dir = ARGV[0]

# 获取默认项目目录，即当前脚本所在目录的上一级
project_dir = File.expand_path('../Build/iOS/', __dir__)
project_path = "#{project_dir}/Unity-iPhone.xcodeproj"

# 定义 .xcframework 文件的路径
xcframeworks = [
  "#{project_dir}/Pods/AmazonPublisherServicesSDK/APS_iOS_SDK-4.7.7/DTBiOSSDK.xcframework",
  "#{project_dir}/Pods/FBAEMKit/XCFrameworks/FBAEMKit.xcframework",
  "#{project_dir}/Pods/FBSDKCoreKit_Basics/XCFrameworks/FBSDKCoreKit_Basics.xcframework",
  "#{project_dir}/Pods/FBSDKCoreKit/XCFrameworks/FBSDKCoreKit.xcframework",
  "#{project_dir}/Pods/FBSDKGamingServicesKit/XCFrameworks/FBSDKGamingServicesKit.xcframework",
  "#{project_dir}/Pods/FBSDKLoginKit/XCFrameworks/FBSDKLoginKit.xcframework",
  "#{project_dir}/Pods/FBSDKShareKit/XCFrameworks/FBSDKShareKit.xcframework",
  "#{project_dir}/Pods/Fyber_Marketplace_SDK/IASDKCore/IASDKCore.xcframework",
  "#{project_dir}/Pods/OpenWrapSDK/OpenWrapSDK/OMSDK_Pubmatic.xcframework",
  "#{project_dir}/Pods/smaato-ios-sdk/vendor/OMSDK_Smaato.xcframework",
  # "#{project_dir}/Pods/Usercentrics/Usercentrics.xcframework"
  # "#{project_dir}/Pods/UsercentricsUI/UsercentricsUI.xcframework"
]

puts "～～～～～～～～～～开始自动修改xcode工程配置～～～～～～～～～～～～～～～～"

# 打开项目文件
project = Xcodeproj::Project.open(project_path)
target = project.targets.find { |item| item.to_s == 'Unity-iPhone' }

raise 'Target not found: Unity-iPhone' unless target

# 查找或创建 'ManuallyAddedFrameworks' 组
manually_added_group = project.main_group.find_subpath('ManuallyAddedFrameworks', true)

if manually_added_group
  puts '发现现有的 ManuallyAddedFrameworks 组，将删除现有引用并重新创建...'
  manually_added_group.clear
else
  manually_added_group = project.main_group.new_group('ManuallyAddedFrameworks', 'ManuallyAddedFrameworks')
  manually_added_group.set_source_tree('<group>')
end

# 查找或创建 'Embed Frameworks' 构建阶段
embed_phase = target.copy_files_build_phases.find { |phase| phase.name == 'Embed Frameworks' } || target.new_copy_files_build_phase('Embed Frameworks')
embed_phase.dst_subfolder_spec = '10'

# 添加框架
xcframeworks.each do |framework_path|
  framework_name = File.basename(framework_path)
  if manually_added_group && manually_added_group.files.none? { |file| file.path == framework_name }
    file_ref = manually_added_group.new_reference(framework_path)
    target.frameworks_build_phase.add_file_reference(file_ref)
    embed_file = embed_phase.add_file_reference(file_ref)
    embed_file.settings = { 'ATTRIBUTES' => ['CodeSignOnCopy'] }
    puts "#{framework_name} 已添加到项目。"
  else
    puts "#{framework_name} 已存在于 ManuallyAddedFrameworks 组中，将被忽略。"
  end
end

# 保存项目文件
project.save
puts '～～～～～～～～～～自动修改xcode工程配置成功～～～～～～～～～～～～～～～～'
