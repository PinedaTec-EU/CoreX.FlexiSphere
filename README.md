# FlexiSphere

FlexiSphere is a C# library (net 9), extensible and configurable component for managing events and scheduled tasks. This project allows you to define triggers and jobs that execute based on certain conditions.

## Features

- **EventTrigger**: Activates when an adhoc implemented logic returns true. It runs every 500ms (configurable).
- **ScheduledTrigger**: Executes at intervals configured according to a cron expression.
- **Jobs**: Configured with at least one job that meets a specific interface and executes when any of the triggers activate.

## Installation

To install FlexiSphere, clone this repository and add the project to your solution:

```bash
git clone https://github.com/PinedaTec-EU/FlexiSphere.git
```

# Contributions
Contributions are welcome! If you want to collaborate, please follow these steps:

Fork the repository.

Create a new branch (git checkout -b feature/new-feature).

Make your changes and commit them (git commit -am 'Add new feature').

Push your changes (git push origin feature/new-feature).

Open a Pull Request.

**License**
This project is licensed under the GPL-3.0. For more details, see the LICENSE file.

**Contact**
For more information, visit the GitHub repository or contact [pinedatec.eu@gmail.com](mailto:pinedatec.eu@gmail.com) with the subject "FlexiSphere:"
