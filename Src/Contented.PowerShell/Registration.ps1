# Import Contented
Import-Module -Name $PSScriptRoot\bin\Debug\netcoreapp2.1\publish\Contented.PowerShell.psd1

$ContentedInputFile = $env:CONTENTED_CONTENT_PATH

## Contented helpers

# edit the contented input file
Function CtdEdit
{
    Notepad $ContentedInputFile
}

# add content from the contented input file
Function CtdAdd
{
    Function AddContent
    {
        $Credentials = GetCachedCredentials
        Add-Contented -Credentials $Credentials -InputFile $ContentedInputFile 
    }

    TryExecute $function:AddContent
}

# list all content
Function CtdList
{
    Function GetContent
    {
        $Credentials = GetCachedCredentials
        Get-Contented -Credentials $Credentials
    }

    TryExecute $function:GetContent
}

# list all completed content
Function CtdListCompleted
{
    Function GetContent
    {
        $OldDate = (Get-Date).AddDays(-31)
        CtdList | Sort-Object -Property Ratio -Descending | Where-Object {$_.Status -eq "completed" -or ($_.Status -eq "seeding" -and $_.Created -le $OldDate)}
    }

    TryExecute $function:GetContent
}

# remove all completed content
Function CtdRemoveCompleted
{
    Function RemoveContent
    {
        $Credentials = GetCachedCredentials
        CtdListCompleted | Remove-Contented
    }

    TryExecute $function:RemoveContent
}

# control download and upload speed
Function CtdControl
{
    Param(
        [Nullable[int]]$MaxDownloadSpeed,
        [Nullable[int]]$MaxUploadSpeed)

    Function ControlContent
    {
        $Credentials = GetCachedCredentials
        Control-Contented -Credentials $Credentials -MaxDownloadSpeed $MaxDownloadSpeed -MaxUploadSpeed $MaxUploadSpeed
    }

    TryExecute $function:ControlContent
}

# rename content
New-Alias CtdRename Rename-Contented

Function GetCachedCredentials
{
    if ($Global:ContentedCredentials -eq $null)
    {
        $Global:ContentedCredentials = Get-Credential
    }

    return $Global:ContentedCredentials
}

Function TryExecute
{
    Param([scriptblock]$functionToExecute)

    try {
        $functionToExecute.Invoke()
    }
    catch {
        $Global:ContentedCredentials = $null
        throw        
    }
}