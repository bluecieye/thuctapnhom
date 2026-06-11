#!/usr/bin/env bash
#
# Đẩy toàn bộ project hiện tại lên repo GitHub trên một branch mới tên "update1".
# Repo đích: https://github.com/bluecieye/thuctapnhom
#
# Cách dùng:
#   chmod +x push-to-update1.sh
#   ./push-to-update1.sh
#
set -euo pipefail

REMOTE_URL="https://github.com/bluecieye/thuctapnhom.git"
BRANCH="update1"

# Chạy script từ thư mục project (thư mục chứa script này)
cd "$(dirname "$0")"

# 1. Khởi tạo git nếu chưa phải là git repo
if [ ! -d .git ]; then
  echo ">> Khởi tạo git repository..."
  git init
fi

# 2. Tạo / chuyển sang branch update1
echo ">> Tạo và chuyển sang branch '$BRANCH'..."
git checkout -B "$BRANCH"

# 3. Thêm remote 'origin' (cập nhật URL nếu đã tồn tại)
if git remote get-url origin >/dev/null 2>&1; then
  echo ">> Cập nhật remote 'origin' -> $REMOTE_URL"
  git remote set-url origin "$REMOTE_URL"
else
  echo ">> Thêm remote 'origin' -> $REMOTE_URL"
  git remote add origin "$REMOTE_URL"
fi

# 4. Add toàn bộ file và commit
echo ">> Thêm và commit các thay đổi..."
git add -A
if git diff --cached --quiet; then
  echo ">> Không có thay đổi nào để commit."
else
  git commit -m "Update project on branch $BRANCH"
fi

# 5. Đẩy lên GitHub
echo ">> Đẩy branch '$BRANCH' lên origin..."
git push -u origin "$BRANCH"

echo ">> Hoàn tất! Branch '$BRANCH' đã được đẩy lên $REMOTE_URL"
