# For Bash or PowerShell
# NOTE you must save with Unix line-endings or you'll get errors like "$'\r': command not found"
echo @'
' > /dev/null
#vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
# Bash Start -----------------------------------------------------------

scriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

pushd $scriptRoot
for dir in `ls -d TestBase*/` ; do dotnet pack $dir ; done
popd

# Bash End -------------------------------------------------------------
# ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
echo > /dev/null <<"out-null" ###
'@ | out-null
#vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
# Powershell Start -----------------------------------------------------

pushd $PSScriptRoot
try
{
    ls -Directory TestBase* | %{ dotnet pack $_.Name }
}
finally{ popd }

# Powershell End -------------------------------------------------------
# ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
out-null

