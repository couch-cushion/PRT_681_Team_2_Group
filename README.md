# PRT_681_Team_2_Group


This repository is used by all members of **PRT681 Team 2** to store weekly study notes, practical tasks, code, research, and other learning evidence.

Each team member should keep their own work inside a separate personal folder.

---

## Repository Structure

The repository should follow this general structure:

```text
PRT_681_Team_2_Group/
│
├── README.md
│
├── StudentName_StudentID/
│   ├── README.md
│   ├── Week_1/
│   │   ├── Study_Notes/
│   │   ├── Task_Practical/
│   │   └── Learned_Material/
│   │
│   ├── Week_2/
│   ├── Week_3/
│   └── ...
│
├── StudentName_StudentID/
├── StudentName_StudentID/
└── StudentName_StudentID/
```

Each student should use a folder name similar to:

```text
FirstNameLastName_StudentID
```

Example:

```text
DeepjanThapaliya_S395373
```

---

# Creating Your Personal Folder

After cloning the repository, go to the repository folder.

Example:

```powershell
git clone https://github.com/couch-cushion/PRT_681_Team_2_Group.git
```

Then:

```powershell
cd PRT_681_Team_2_Group
```

Before starting new work, update your local repository:

```powershell
git switch main
git pull origin main
```

Create your personal folder:

```powershell
New-Item -ItemType Directory -Force -Path "YourName_StudentID"
```

Example:

```powershell
New-Item -ItemType Directory -Force -Path "DeepjanThapaliya_S395373"
```

---

# Creating Weekly Folders

Inside your personal folder, create a folder for each week.

Recommended structure:

```text
YourName_StudentID/
└── Week_1/
    ├── Study_Notes/
    ├── Task_Practical/
    └── Learned_Material/
```

Example PowerShell commands:

```powershell
$studentFolder = "YourName_StudentID"
$weekFolder = "$studentFolder/Week_1"

New-Item -ItemType Directory -Force -Path "$weekFolder/Study_Notes"
New-Item -ItemType Directory -Force -Path "$weekFolder/Task_Practical"
New-Item -ItemType Directory -Force -Path "$weekFolder/Learned_Material"
```

### Folder Purpose

**Study_Notes**

Use this folder for:

* Lecture notes
* Weekly learning notes
* Summaries
* Important concepts
* Notes from provided learning materials

**Task_Practical**

Use this folder for:

* Weekly tasks
* Programming exercises
* Practical projects
* C#/.NET code
* Testing exercises
* Other required practical work

**Learned_Material**

Use this folder for:

* Additional research
* Extra practice
* Useful commands
* Git notes
* Development notes
* Additional concepts learned during the week

---

# Important Git Rule

## Do NOT run `git init` inside your personal folder

The whole project is already one Git repository.

Do not create another Git repository inside:

```text
YourName_StudentID/
```

Do not create `.git` folders inside individual student folders or practical projects.

All student work should be tracked by the main:

```text
PRT_681_Team_2_Group
```

repository.

---

# Recommended Branch Workflow

Each member should work on their own branch instead of directly changing `main`.

Before starting:

```powershell
git switch main
git pull origin main
```

Create your branch:

```powershell
git switch -c yourname/week-1
```

Example:

```powershell
git switch -c deepjan/week-1
```

After completing some meaningful work:

```powershell
git status
```

Stage only your own folder:

```powershell
git add -- "YourName_StudentID"
```

Check what is staged:

```powershell
git --no-pager diff --cached --name-only
```

Commit:

```powershell
git commit -m "Add Week 1 study notes and practical work"
```

Push your branch:

```powershell
git push -u origin yourname/week-1
```

Then create a **Pull Request** on GitHub to merge your branch into:

```text
main
```

---

# Before Starting a New Week

Always update your local `main` branch first:

```powershell
git switch main
git pull origin main
```

Then create a new branch:

```powershell
git switch -c yourname/week-2
```

Example:

```powershell
git switch -c deepjan/week-2
```

Then create:

```text
YourName_StudentID/
└── Week_2/
    ├── Study_Notes/
    ├── Task_Practical/
    └── Learned_Material/
```

Continue the same structure for future weeks.

---

# Good Commit Messages

Use commit messages that explain what was actually added or changed.

Good examples:

```text
Add Week 1 software engineering study notes
```

```text
Add Week 1 C# practical exercises
```

```text
Document Week 1 Git learning
```

```text
Add validation practice to Week 2 tasks
```

Avoid unclear messages such as:

```text
update
```

```text
changes
```

```text
work
```

```text
week 1
```

---

# Team Rules

1. Keep your work inside your own student folder.
2. Do not modify another member's folder unless you have discussed it with them.
3. Do not create nested Git repositories.
4. Pull the latest `main` branch before starting new work.
5. Use your own branch for weekly work.
6. Use meaningful commit messages.
7. Review staged files before committing.
8. Push your branch and use a Pull Request when merging into `main`.
9. Keep weekly work organised and easy to understand.
10. Ask another team member before making changes to shared repository files.

---

# Need Help?

If you are unsure about the repository structure or Git commands, ask another team member before running commands that may affect shared work.

Before committing, you can safely check:

```powershell
git status
```

and:

```powershell
git --no-pager diff --cached --name-only
```

These commands help confirm which files are being tracked or committed.

The goal is to keep the repository clean, organised, and easy for every team member to use.
