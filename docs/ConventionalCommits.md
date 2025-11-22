# Conventional Commits 提交规范

本文档详细说明了 BinggoWallpapers.WinUI 项目所采用的 Conventional Commits（约定式提交）规范，以确保提交历史清晰、规范且易于追踪。

---

## 📖 什么是 Conventional Commits

Conventional Commits 是一种用于规范化提交信息的轻量级约定。它提供了一套简单的规则来创建清晰的提交历史，使得自动化工具能够轻松地从提交历史中生成变更日志（CHANGELOG）。

**官方规范**：[conventionalcommits.org](https://www.conventionalcommits.org/)

---

## 🎯 为什么使用 Conventional Commits

### 优势

-   ✅ **自动化版本管理**：根据提交类型自动确定语义化版本号
-   ✅ **自动生成 CHANGELOG**：从提交历史自动生成更新日志
-   ✅ **清晰的提交历史**：让团队成员和贡献者快速理解每次提交的目的
-   ✅ **触发构建和发布流程**：基于提交类型触发特定的 CI/CD 流程
-   ✅ **更容易的代码审查**：审查者能快速识别改动性质
-   ✅ **更好的协作体验**：统一的提交规范减少沟通成本

---

## 📝 提交信息格式

### 基本结构

```
<type>(<scope>): <subject>

<body>

<footer>
```

### 详细说明

#### 1. Header（标题行）- **必需**

```
<type>(<scope>): <subject>
```

-   **type**：提交类型（必需）
-   **scope**：影响范围（可选）
-   **subject**：简短描述（必需）

**规则**：

-   标题行总长度不超过 72 个字符
-   subject 使用祈使句，首字母小写，结尾不加句号
-   subject 应该清晰描述本次提交的目的

#### 2. Body（正文）- 可选

-   详细描述本次提交的动机和实现思路
-   与 header 之间空一行
-   可以分多段落
-   每行不超过 100 个字符

#### 3. Footer（脚注）- 可选

-   用于关闭 Issue 或描述破坏性变更
-   与 body 之间空一行

---

## 🏷️ Type（提交类型）

### 主要类型

| 类型       | Emoji | 说明                                     | 影响版本 |
| ---------- | ----- | ---------------------------------------- | -------- |
| `feat`     | ✨    | 新增功能（feature）                      | MINOR    |
| `fix`      | 🐛    | 修复 Bug                                 | PATCH    |
| `docs`     | 📝    | 文档变更                                 | -        |
| `style`    | 💄    | 代码格式调整（不影响代码逻辑）           | -        |
| `refactor` | ♻️    | 代码重构（既不是新增功能也不是修复 Bug） | -        |
| `perf`     | ⚡    | 性能优化                                 | PATCH    |
| `test`     | ✅    | 测试相关（新增或修改测试）               | -        |
| `build`    | 📦    | 构建系统或外部依赖变更                   | -        |
| `ci`       | 👷    | CI 配置文件和脚本变更                    | -        |
| `chore`    | 🔧    | 其他不修改 src 或测试文件的变更          | -        |
| `revert`   | ⏪    | 回滚之前的提交                           | -        |

### 类型详解

#### ✨ `feat` - 新功能

新增任何用户可见的功能或特性。

```
feat(gallery): 添加壁纸收藏功能
feat(editor): 支持新的锐化特效
feat: 添加多语言支持
```

#### 🐛 `fix` - Bug 修复

修复任何影响用户体验的 Bug。

```
fix(database): 修复壁纸数据重复插入问题
fix(ui): 修复图片加载失败时的崩溃问题
fix: 解决 Windows 11 下主题切换异常
```

#### 📝 `docs` - 文档

修改文档、注释、README 等。

```
docs: 更新快速开始指南
docs(api): 完善 WallpaperService 的 XML 注释
docs: 添加 Conventional Commits 规范文档
```

#### 💄 `style` - 代码格式

不影响代码运行的格式调整，如空格、缩进、换行等。

```
style: 统一代码缩进为 4 个空格
style(viewmodel): 调整代码对齐格式
style: 移除多余的空行
```

#### ♻️ `refactor` - 重构

既不修复 Bug 也不添加功能的代码重构。

```
refactor(service): 提取通用的 HTTP 请求逻辑
refactor: 将硬编码的常量提取到配置文件
refactor(viewmodel): 优化命令绑定实现
```

#### ⚡ `perf` - 性能优化

提升性能的代码修改。

```
perf(gallery): 优化图片加载性能
perf: 使用缓存减少网络请求
perf(database): 添加数据库索引提升查询速度
```

#### ✅ `test` - 测试

添加、修改或删除测试代码。

```
test: 添加 WallpaperService 单元测试
test(repository): 增加边界条件测试用例
test: 提升测试覆盖率到 80%
```

#### 📦 `build` - 构建系统

影响构建系统或外部依赖的更改。

```
build: 升级 .NET 到 9.0
build: 更新 WinUI 依赖到 1.8.0
build: 修改 NuGet 包管理配置
```

#### 👷 `ci` - 持续集成

修改 CI 配置文件和脚本。

```
ci: 添加代码覆盖率检查
ci: 优化 GitHub Actions 工作流
ci: 配置自动发布到 Microsoft Store
```

#### 🔧 `chore` - 其他杂项

不属于以上类型的其他修改。

```
chore: 更新 .gitignore 文件
chore: 清理无用的依赖项
chore: 更新许可证文件
```

---

## 🎯 Scope（影响范围）

Scope 用于说明本次提交影响的范围，应该是项目中具体的模块或组件名称。

### 项目中的常用 Scope

| Scope       | 说明           |
| ----------- | -------------- |
| `ui`        | 用户界面相关   |
| `gallery`   | 壁纸画廊模块   |
| `editor`    | 图片编辑器模块 |
| `database`  | 数据库相关     |
| `service`   | 服务层         |
| `viewmodel` | ViewModel 层   |
| `model`     | 数据模型       |
| `core`      | 核心功能模块   |
| `collector` | 数据收集器     |
| `config`    | 配置相关       |
| `deps`      | 依赖项管理     |

### Scope 使用示例

```
feat(gallery): 添加按日期筛选功能
fix(editor): 修复特效预览卡顿问题
refactor(service): 重构 HTTP 请求处理
test(repository): 添加数据仓储单元测试
docs(config): 完善配置文件说明
```

---

## 💡 示例

### 基本示例

#### 简单的新功能

```
feat: 添加壁纸分享功能
```

#### 带 Scope 的 Bug 修复

```
fix(gallery): 修复壁纸缩略图加载失败问题
```

#### 带详细描述的提交

```
feat(editor): 添加水印功能

- 支持文本水印和图片水印
- 可自定义水印位置和透明度
- 支持实时预览

Closes #123
```

### 完整示例

#### 新功能提交

```
feat(gallery): 支持壁纸按地区筛选

添加地区筛选下拉菜单，用户可以快速切换不同国家/地区的壁纸。
支持的地区包括：中国、美国、日本、德国等 14 个国家。

实现细节：
- 在 HomeViewModel 中添加 FilterByRegion 方法
- 新增 RegionFilterControl 用户控件
- 更新数据库查询逻辑以支持地区筛选

Closes #45
```

#### Bug 修复提交

```
fix(database): 修复并发访问导致的数据库锁定问题

在高并发场景下，多个线程同时访问数据库会导致 SQLite 数据库锁定。
通过引入连接池和重试机制解决此问题。

修改内容：
- 配置 SQLite 连接池
- 添加数据库操作重试逻辑（最多 3 次）
- 增加数据库访问超时时间到 30 秒

Fixes #78
```

#### 破坏性变更提交

```
feat(api)!: 重构壁纸数据模型

BREAKING CHANGE: WallpaperInfo 类的属性名称已更改

旧属性：
- ImageUrl -> Url
- ImageTitle -> Title
- ImageDescription -> Description

新属性遵循更简洁的命名规范。
需要更新所有引用 WallpaperInfo 的代码。

Migration Guide: 参见 docs/migration-v2.md

Closes #156
```

#### 回滚提交

```
revert: 回滚 "feat(gallery): 添加壁纸分享功能"

由于在 Windows 10 上存在兼容性问题，暂时回滚此功能。

This reverts commit a1b2c3d4e5f6.
```

---

## 🚀 破坏性变更（Breaking Changes）

### 标记方式

破坏性变更可以通过以下两种方式标记：

#### 方式一：在 type 后添加 `!`

```
feat(api)!: 修改 API 响应格式
refactor!: 重命名核心类和方法
```

#### 方式二：在 footer 中使用 `BREAKING CHANGE:`

```
feat(config): 更新配置文件格式

BREAKING CHANGE: 配置文件格式从 XML 迁移到 JSON

旧格式将不再支持，请使用新的 JSON 配置文件。
迁移工具：tools/migrate-config.ps1
```

### 何时使用

-   修改公共 API
-   删除已有功能
-   更改数据格式或协议
-   需要用户手动迁移的变更

---

## 🔗 Footer 关键字

### 关联 Issue

```
Closes #123
Fixes #456
Resolves #789
Closes #123, #456, #789
```

### 其他 Footer

```
Refs #123
See also #456
Related to #789
```

### 审查者标记

```
Reviewed-by: Name <email@example.com>
Acked-by: Name <email@example.com>
Co-authored-by: Name <email@example.com>
```

---

## ✅ 最佳实践

### 提交原则

1.  **原子性提交**：每次提交只做一件事
2.  **独立性**：每个提交都应该能够独立工作
3.  **完整性**：提交应该包含完整的功能或修复
4.  **可测试性**：每次提交后代码应该能通过测试

### 提交频率

-   ✅ **多次小提交** 优于 **一次大提交**
-   ✅ 功能开发过程中频繁提交
-   ✅ 每个逻辑单元完成后立即提交
-   ❌ 不要等到一天结束时才提交所有改动

### Subject 编写技巧

**✅ 好的示例**

```
feat: 添加夜间模式
fix: 解决图片加载超时问题
refactor: 简化配置加载逻辑
```

**❌ 不好的示例**

```
feat: 添加了一些功能
fix: 修复 Bug
update: 更新代码
```

### 何时写 Body

以下情况建议添加详细的 body：

-   **复杂的变更**：实现逻辑复杂，需要解释
-   **重要决策**：说明为什么这样做而不是那样做
-   **有多个改动**：列出所有修改点
-   **修复 Bug**：说明 Bug 的原因和修复方法
-   **破坏性变更**：详细说明影响和迁移方法

---

## 🛠️ 工具推荐

### 本地 Git Hook 配置（推荐）

对于 .NET 项目，推荐使用原生的 Git Hooks，无需安装 Node.js 依赖。

#### 方法一：手动创建 commit-msg Hook

在项目根目录执行以下命令：

**Windows (PowerShell)**

```powershell
# 创建 .git/hooks 目录（如果不存在）
New-Item -ItemType Directory -Force -Path .git/hooks

# 创建 commit-msg hook
@'
#!/bin/sh
# Conventional Commits 验证脚本

commit_msg_file=$1
commit_msg=$(cat "$commit_msg_file")

# 定义颜色
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Conventional Commits 正则表达式
# 格式: type(scope): subject 或 type(scope)!: subject
pattern="^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?(!)?: .{1,72}"

if echo "$commit_msg" | grep -qE "$pattern"; then
    echo "${GREEN}✓ 提交信息格式正确${NC}"
    exit 0
else
    echo "${RED}✗ 提交信息格式不符合 Conventional Commits 规范${NC}"
    echo ""
    echo "${YELLOW}正确格式:${NC}"
    echo "  <type>(<scope>): <subject>"
    echo ""
    echo "${YELLOW}示例:${NC}"
    echo "  feat(gallery): 添加壁纸收藏功能"
    echo "  fix(database): 修复数据重复插入问题"
    echo "  docs: 更新 README 文档"
    echo ""
    echo "${YELLOW}支持的 type:${NC}"
    echo "  ✨ feat     - 新增功能"
    echo "  🐛 fix      - Bug 修复"
    echo "  📝 docs     - 文档变更"
    echo "  💄 style    - 代码格式"
    echo "  ♻️  refactor - 代码重构"
    echo "  ⚡ perf     - 性能优化"
    echo "  ✅ test     - 测试"
    echo "  📦 build    - 构建系统"
    echo "  👷 ci       - CI 配置"
    echo "  🔧 chore    - 其他杂项"
    echo "  ⏪ revert   - 回滚提交"
    echo ""
    echo "${YELLOW}您的提交信息:${NC}"
    echo "  $commit_msg"
    exit 1
fi
'@ | Out-File -Encoding UTF8 .git/hooks/commit-msg

# 在 Windows 上，Git 会自动处理可执行权限
```

**macOS / Linux (Bash)**

```bash
# 创建 commit-msg hook
cat > .git/hooks/commit-msg << 'EOF'
#!/bin/sh
# Conventional Commits 验证脚本

commit_msg_file=$1
commit_msg=$(cat "$commit_msg_file")

# 定义颜色
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Conventional Commits 正则表达式
# 格式: type(scope): subject 或 type(scope)!: subject
pattern="^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?(!)?: .{1,72}"

if echo "$commit_msg" | grep -qE "$pattern"; then
    echo "${GREEN}✓ 提交信息格式正确${NC}"
    exit 0
else
    echo "${RED}✗ 提交信息格式不符合 Conventional Commits 规范${NC}"
    echo ""
    echo "${YELLOW}正确格式:${NC}"
    echo "  <type>(<scope>): <subject>"
    echo ""
    echo "${YELLOW}示例:${NC}"
    echo "  feat(gallery): 添加壁纸收藏功能"
    echo "  fix(database): 修复数据重复插入问题"
    echo "  docs: 更新 README 文档"
    echo ""
    echo "${YELLOW}支持的 type:${NC}"
    echo "  ✨ feat     - 新增功能"
    echo "  🐛 fix      - Bug 修复"
    echo "  📝 docs     - 文档变更"
    echo "  💄 style    - 代码格式"
    echo "  ♻️  refactor - 代码重构"
    echo "  ⚡ perf     - 性能优化"
    echo "  ✅ test     - 测试"
    echo "  📦 build    - 构建系统"
    echo "  👷 ci       - CI 配置"
    echo "  🔧 chore    - 其他杂项"
    echo "  ⏪ revert   - 回滚提交"
    echo ""
    echo "${YELLOW}您的提交信息:${NC}"
    echo "  $commit_msg"
    exit 1
fi
EOF

# 添加可执行权限
chmod +x .git/hooks/commit-msg
```

#### 方法二：使用配置脚本

项目已提供配置脚本（位于 `scripts/` 目录），可直接运行：

**Windows 用户**

```powershell
# 使用 PowerShell 运行
.\scripts\setup-git-hooks.ps1

# 或者在项目根目录
pwsh scripts/setup-git-hooks.ps1
```

**macOS / Linux 用户**

```bash
# 运行配置脚本
./scripts/setup-git-hooks.sh

# 或者
bash scripts/setup-git-hooks.sh
```

---

#### 脚本内容参考

如果需要自定义或了解脚本内容：

**scripts/setup-git-hooks.ps1 (Windows)**

```powershell
#!/usr/bin/env pwsh
# Git Hooks 配置脚本

Write-Host "🔧 正在配置 Git Hooks..." -ForegroundColor Cyan

$hooksDir = ".git/hooks"
$commitMsgHook = "$hooksDir/commit-msg"

# 确保 hooks 目录存在
if (-not (Test-Path $hooksDir)) {
    New-Item -ItemType Directory -Force -Path $hooksDir | Out-Null
}

# 创建 commit-msg hook
@'
#!/bin/sh
# Conventional Commits 验证脚本

commit_msg_file=$1
commit_msg=$(cat "$commit_msg_file")

# 定义颜色
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Conventional Commits 正则表达式
pattern="^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?(!)?: .{1,72}"

if echo "$commit_msg" | grep -qE "$pattern"; then
    echo "${GREEN}✓ 提交信息格式正确${NC}"
    exit 0
else
    echo "${RED}✗ 提交信息格式不符合 Conventional Commits 规范${NC}"
    echo ""
    echo "${YELLOW}正确格式: <type>(<scope>): <subject>${NC}"
    echo ""
    echo "${YELLOW}示例:${NC}"
    echo "  feat(gallery): 添加壁纸收藏功能"
    echo "  fix(database): 修复数据重复插入问题"
    echo ""
    echo "${YELLOW}详细说明请查看: docs/ConventionalCommits.md${NC}"
    exit 1
fi
'@ | Out-File -Encoding UTF8 $commitMsgHook

Write-Host "✅ Git Hooks 配置完成！" -ForegroundColor Green
Write-Host ""
Write-Host "现在您的每次提交都会自动验证提交信息格式。" -ForegroundColor Gray
Write-Host "详细规范请查看: docs/ConventionalCommits.md" -ForegroundColor Gray
```

**scripts/setup-git-hooks.sh (macOS/Linux)**

```bash
#!/bin/bash
# Git Hooks 配置脚本

echo "🔧 正在配置 Git Hooks..."

HOOKS_DIR=".git/hooks"
COMMIT_MSG_HOOK="$HOOKS_DIR/commit-msg"

# 确保 hooks 目录存在
mkdir -p "$HOOKS_DIR"

# 创建 commit-msg hook
cat > "$COMMIT_MSG_HOOK" << 'EOF'
#!/bin/sh
# Conventional Commits 验证脚本

commit_msg_file=$1
commit_msg=$(cat "$commit_msg_file")

# 定义颜色
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Conventional Commits 正则表达式
pattern="^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?(!)?: .{1,72}"

if echo "$commit_msg" | grep -qE "$pattern"; then
    echo "${GREEN}✓ 提交信息格式正确${NC}"
    exit 0
else
    echo "${RED}✗ 提交信息格式不符合 Conventional Commits 规范${NC}"
    echo ""
    echo "${YELLOW}正确格式: <type>(<scope>): <subject>${NC}"
    echo ""
    echo "${YELLOW}示例:${NC}"
    echo "  feat(gallery): 添加壁纸收藏功能"
    echo "  fix(database): 修复数据重复插入问题"
    echo ""
    echo "${YELLOW}详细说明请查看: docs/ConventionalCommits.md${NC}"
    exit 1
fi
EOF

# 添加可执行权限
chmod +x "$COMMIT_MSG_HOOK"

echo "✅ Git Hooks 配置完成！"
echo ""
echo "现在您的每次提交都会自动验证提交信息格式。"
echo "详细规范请查看: docs/ConventionalCommits.md"
```

#### 使用方法

配置完成后，每次执行 `git commit` 时都会自动验证提交信息格式：

```bash
# 正确的提交会通过验证
git commit -m "feat(gallery): 添加壁纸收藏功能"
# ✓ 提交信息格式正确

# 错误的提交会被拦截
git commit -m "added new feature"
# ✗ 提交信息格式不符合 Conventional Commits 规范
```

#### 临时跳过验证

如果需要临时跳过 hook 验证（不推荐）：

```bash
git commit --no-verify -m "your message"
```

---

### 基于 Node.js 的工具（可选）

如果项目中已经使用 Node.js，可以使用以下工具：

#### commitlint

安装和配置 commitlint 来验证提交信息格式：

```bash
# 安装 commitlint
npm install --save-dev @commitlint/cli @commitlint/config-conventional

# 创建配置文件
echo "module.exports = {extends: ['@commitlint/config-conventional']}" > commitlint.config.js

# 使用 husky 配置 Git Hooks
npm install --save-dev husky
npx husky install
npx husky add .husky/commit-msg 'npx --no -- commitlint --edit "$1"'
```

#### Commitizen

使用交互式命令行工具生成规范的提交信息：

```bash
# 安装 Commitizen
npm install --save-dev commitizen

# 初始化
npx commitizen init cz-conventional-changelog --save-dev --save-exact

# 使用（替代 git commit）
npx cz
```

### Visual Studio Code 扩展

-   **Conventional Commits**：提供提交模板和自动补全
-   **Git Commit Template**：自定义提交模板
-   **GitLens**：可视化 Git 提交历史

### GitHub Apps

-   **Semantic Pull Requests**：验证 PR 标题是否符合规范
-   **Release Drafter**：根据提交自动生成 Release Notes

---

## 📊 版本号影响

根据 [语义化版本（Semantic Versioning）](https://semver.org/)：

| 提交类型                | 版本影响      | 示例          |
| ----------------------- | ------------- | ------------- |
| `feat`                  | MINOR (x.Y.z) | 1.2.0 → 1.3.0 |
| `fix`, `perf`           | PATCH (x.y.Z) | 1.2.0 → 1.2.1 |
| `BREAKING CHANGE` / `!` | MAJOR (X.y.z) | 1.2.0 → 2.0.0 |
| 其他类型                | 不影响        | -             |

---

## 🎓 实战练习

### 场景 1：添加新功能

**任务**：在壁纸编辑器中添加了亮度调整功能

```
feat(editor): 添加亮度调整功能

用户现在可以调整壁纸的亮度（-100 到 +100）。
使用 Win2D 的 BrightnessEffect 实现，支持实时预览。

Closes #234
```

### 场景 2：修复 Bug

**任务**：修复了壁纸下载失败后应用崩溃的问题

```
fix(download): 修复下载失败时的空指针异常

当网络请求超时时，Response 对象可能为 null，导致程序崩溃。
添加空值检查和错误处理逻辑。

Fixes #567
```

### 场景 3：重构代码

**任务**：重构了数据库访问层，使用仓储模式

```
refactor(database): 使用仓储模式重构数据访问层

- 创建 IRepository<T> 接口和 Repository<T> 基类
- 将 ApplicationDbContext 的直接访问改为通过仓储
- 提高代码可测试性和可维护性

Related to #890
```

### 场景 4：破坏性变更

**任务**：修改了配置文件格式，从 XML 改为 JSON

```
feat(config)!: 配置文件格式从 XML 迁移到 JSON

BREAKING CHANGE: appsettings.xml 不再支持

新版本使用 appsettings.json 作为配置文件。
应用会自动尝试迁移旧配置，但建议手动检查。

迁移步骤：
1. 备份 appsettings.xml
2. 运行应用，会自动生成 appsettings.json
3. 验证配置是否正确
4. 删除旧的 XML 文件

Closes #1001
```

---

## 📚 参考资源

-   [Conventional Commits 官方规范](https://www.conventionalcommits.org/)
-   [Angular 提交规范](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#commit)
-   [Semantic Versioning](https://semver.org/)
-   [commitlint](https://commitlint.js.org/)
-   [Commitizen](https://commitizen-tools.github.io/commitizen/)

---

## 💬 常见问题

### Q1: 一次提交包含多种类型的修改怎么办？

**A**: 应该拆分成多个独立的提交。如果确实无法拆分，使用最主要的类型。

### Q2: Scope 是必需的吗？

**A**: 不是必需的，但建议在有明确影响范围时添加，这样可以更清晰地了解改动位置。

### Q3: 提交信息应该用中文还是英文？

**A**: 本项目推荐使用**中文**，保持与文档和注释的一致性。但 type 和 scope 使用英文。

### Q4: 什么情况下需要添加 Body？

**A**: 当 subject 无法完整表达改动的原因、实现细节或影响范围时，应该添加 body。

### Q5: 如何处理临时提交或 WIP 提交？

**A**:

-   开发分支可以使用 `wip: 功能开发中`
-   合并到主分支前应该使用 `git rebase -i` 整理成规范的提交
-   或者使用 squash merge 将多个临时提交合并为一个规范的提交

### Q6: 忘记写规范的提交信息怎么办？

**A**: 使用 `git commit --amend` 修改最后一次提交，或使用 `git rebase -i` 修改历史提交。

---

**最后更新时间**：2025-10-19
