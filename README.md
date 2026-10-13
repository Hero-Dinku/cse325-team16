
A collaborative task tracker for student study groups.

StudySync helps college students who work on group projects and exam prep stay organized. Students create or join a study group, then share a task list with due dates, assignees, and progress status, all in one place instead of scattered text threads and spreadsheets.

Built for CSE 325: .NET Software Development (BYU-Pathway) by Team 16.

 Team Members

- Asmamaw Yesgat Dinku (Group Leader)
- Daniel Mark Koepke
- Andrea Natalia Munoz Baca
- Mirian Ukeje

 Project Links

- GitHub repository: https://github.com/Hero-Dinku/cse325-team16
- Trello board: https://trello.com/b/EYOVoEQs/team-16-studysync

 Features

- User accounts: register, log in, and log out with ASP.NET Core Identity. Pages that hold group data require login.
- Study groups: create a group and get an auto-generated 6-character join code; join an existing group by entering its code.
- Task management (full CRUD): create, view, edit, and delete tasks inside a group.
- Assignments and due dates: assign a task to any group member, set a due date, and see an "Overdue" badge on late tasks.
- Status tracking: mark tasks as Not started, In progress, or Completed from a dropdown.
- Access control: only group members can see or change a group's tasks. Only a task's creator or the group's creator can delete it.
- Validation and error handling: form validation messages, friendly error banners, and clear messages for wrong join codes, missing groups, and non-members.
- Responsive, accessible design: a layout that works on phones, tablets, and desktops, with labeled form fields, a captioned task table, and ARIA roles on status messages.

 Planned / not yet included

- Comments on tasks (the data model exists; the interface is not built yet)
- Group activity feed
- File attachments and calendar integrations

 Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 8 (LTS), Blazor Server (interactive server rendering) |
| Authentication | ASP.NET Core Identity |
| Data access | Entity Framework Core 8 |
| Database | SQLite |
| Styling | Bootstrap (from the Blazor template) plus custom CSS |

 Getting Started

 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (the repo's `global.json` pins .NET 8)
- Git
- The EF Core command-line tool:

```powershell
dotnet tool install --global dotnet-ef
```

 Run it locally

```powershell
git clone https://github.com/Hero-Dinku/cse325-team16.git
cd cse325-team16\StudySync
dotnet ef database update
dotnet run
```

Open the address printed in the terminal (for example `http://localhost:5145`).

The local SQLite database file is created by `dotnet ef database update` and is intentionally not committed to Git, so each developer runs this command once on their own machine.

 User Guide

1. Register. Click Register in the left menu, enter an email and password, and submit. Then log in.
2. Create a group. Open My Groups, fill in the "Create a New Group" form, and submit. Your new group appears in the list with its join code.
3. Invite classmates. Share the join code. They log in, open My Groups, enter the code under "Join an Existing Group", and submit.
4. Open a group. Click the group's name to see its members and tasks.
5. Add a task. Enter a title (required), an optional description and due date, choose a status, and pick who it's assigned to. Click Add Task.
6. Update progress. Use the status dropdown in the task table to move a task between Not started, In progress, and Completed.
7. Edit a task. Click Edit, change the fields in the form, and click Save Changes (or Cancel).
8. Delete a task. Click Delete, then Confirm delete. Only the task's creator or the group's creator can delete a task.

 Project Structure

```
cse325-team16/
├── global.json                   Pins the .NET 8 SDK
├── README.md
└── StudySync/
    ├── Program.cs                App startup, service registration
    ├── Data/                     ApplicationDbContext and EF Core migrations
    ├── Models/                   StudyGroup, GroupMembership, TaskItem, Comment
    ├── Services/                 StudyGroupService, TaskService (business logic)
    └── Components/
        ├── Layout/               Main layout and navigation menu
        ├── Pages/                Home, MyGroups, GroupDetail, Identity pages
        └── Account/              Identity (login/register) components
```

 Design notes

- Service layer: database logic lives in `StudyGroupService` and `TaskService`, not in the pages. Each method checks group membership and returns a success flag plus a message the page can show to the user.
- Data model: a `GroupMembership` join table links users to groups (with a unique index so nobody joins twice). Deleting a group cascades to its tasks, and deleting a task cascades to its comments.

 Team Workflow

- Project management: tasks tracked on our Trello board (link above).
- Source control: GitHub repository with regular commits and a `.gitignore` that keeps build output and local databases out of the repo.
- Meetings: weekly on Microsoft Teams, with a rotating group leader.
