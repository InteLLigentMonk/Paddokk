# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade Paddokk.Core\Paddokk.Core.csproj to .NET 10.0
4. Upgrade Paddokk.Data\Paddokk.Data.csproj to .NET 10.0
5. Upgrade Paddokk.Api\Paddokk.Api.csproj to .NET 10.0

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                 | Current Version | New Version | Description   |
|:----------------------------------------------------------------------|:---------------:|:-----------:|:-----------------------------------|
| Microsoft.AspNetCore.Authentication.JwtBearer      |    9.0.6        |   10.0.3    | Recommended for .NET 10.0   |
| Microsoft.AspNetCore.OpenApi          |  9.0.2   |   10.0.3    | Recommended for .NET 10.0   |
| Microsoft.EntityFrameworkCore   |    9.0.6  |   10.0.3    | Recommended for .NET 10.0          |
| Microsoft.EntityFrameworkCore.Design              |    9.0.6        |   10.0.3    | Recommended for .NET 10.0   |
| Microsoft.EntityFrameworkCore.SqlServer       | 9.0.6      |   10.0.3 | Recommended for .NET 10.0     |
| Microsoft.EntityFrameworkCore.Tools             |    9.0.6        |   10.0.3    | Recommended for .NET 10.0          |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore    |    9.0.6        |   10.0.3    | Recommended for .NET 10.0          |

### Project upgrade details

#### Paddokk.Core\Paddokk.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `9.0.6` to `10.0.3`
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.2` to `10.0.3`
  - Microsoft.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Tools should be updated from `9.0.6` to `10.0.3`
  - Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`

#### Paddokk.Data\Paddokk.Data.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Tools should be updated from `9.0.6` to `10.0.3`
  - Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`

#### Paddokk.Api\Paddokk.Api.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `9.0.6` to `10.0.3`
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.2` to `10.0.3`
  - Microsoft.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.0.6` to `10.0.3`
  - Microsoft.EntityFrameworkCore.Tools should be updated from `9.0.6` to `10.0.3`
  - Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore should be updated from `9.0.6` to `10.0.3`
