---
create_date: 2015-12
archive_date: 2016-11
language: C#
framework: .NET Framework
locale: en_US
editor: Visual Studio Community 2015+
license: MIT
category: [PoC, Study, Utility, Desktop]
dev_status: Archive
---

# Description

LicenseServer is simple license management system with client-server architecture in mind. The purpose of this software is to serve central system with licenses database and provide desktop client application with ability to view, create, modify and remove licenses as well as users and registered software.

Client app allows users with different provileges to view own licenses. Administrators can add software, link software license to user and change various aspects like expiration time.

The core of communication is build using SOAP transport layer protocol and Windows Communication Foundation framework - a part of .NET Framework on Windows platform. Client-side applications takes the best from Windows Presentation Foundation framework.

# Setup

1. Open project in Visual Studio.
2. Right click on the Solution in Solution Explorer and click "Restore NuGet Packages".
3. Right click again and choose "Set StartUp Projects...".
4. Set action to "Start" for "KonsolaLicencji" and "Wielowarstwowa" so you can just start both server and client with a single click.
5. Run the project in Visual Studio.

# Screenshots

| Client Window | Server Console |
--- | ---
![Manager Window](docs/screenshots/manager.png) | ![Server Console](docs/screenshots/console.png)
![Client Window Connected](docs/screenshots/client_window_software.png) | ![Client Window Connected](docs/screenshots/client_window_users.png)


||||
--- | --- | ---
![User List](docs/screenshots/manager_users.png) | ![Software List](docs/screenshots/manager_software.png) | ![License List](docs/screenshots/manager_licenses.png)