# For Bash or PowerShell
# NOTE you must save with Unix line-endings or you'll get errors like "$'\r': command not found"
echo @'
' > /dev/null
#vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
# Bash Start -----------------------------------------------------------

scriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

pushd $scriptRoot
find . -iname *.nupkg -delete
for dir in `ls -d TestBase*/` ; do msbuild -t:pack $dir ; done
msbuild -t:pack PredicateDictionary
msbuild -t:pack TooString
msbuild -t:pack Extensions.Logging.ListOfString
msbuild -t:pack Serilog.Sinks.LostOfString
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
  Get-ChildItem -rec *.nupkg | ForEach-Object{ Remove-Item $_  }
  Get-ChildItem -Directory -Include TestBase,
                          TooString,
                          TestBase.RecordingStopwatch,
                          PredicateDictionary, 
                          Extensions.Logging.ListOfString,
                          Serilog.Sinks.LostOfString | ForEach-Object{ 

    msbuild -t:pack $_.Name

  }
}
finally{ popd }

# Powershell End -------------------------------------------------------
# ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
out-null

