# For Bash or PowerShell
# NOTE you must save with Unix line-endings or you'll get errors like "$'\r': command not found"
echo @'
' > /dev/null
#vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
# Bash Start -----------------------------------------------------------

scriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

pushd $scriptRoot
find . -iname *.nupkg -delete
for dir in `ls -d TestBase*/` ; do dotnet pack $dir ; done
dotnet pack PredicateDictionary
dotnet pack TooString
dotnet pack Extensions.Logging.ListOfString
dotnet pack Serilog.Sinks.LostOfString
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
  gci -rec *.nupkg | %{ rm $_ }
  gci -Directory TestBase* | %{ dotnet pack $_.Name }
  dotnet pack PredicateDictionary
  dotnet pack TooString
  dotnet pack Extensions.Logging.ListOfString
  dotnet pack Serilog.Sinks.LostOfString  
}
finally{ popd }

# Powershell End -------------------------------------------------------
# ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
out-null

