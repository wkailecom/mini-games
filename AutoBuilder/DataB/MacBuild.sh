#!/bin/bash

# 目标目录
SCRIPT_DIR="$(dirname "$0")"
# 移动到目标目录
cd "$SCRIPT_DIR/main" || exit

echo "$(python3 --version)"
# 执行 Python 脚本
python3 Main.py $1 $2
