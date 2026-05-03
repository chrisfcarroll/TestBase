# Serilog.Assert( it is "A better style of application logging" ).
```
public string GoBang(string target, int guess)
{
    log.Member( (target,guess) );
    log.PreconditionNotNull(target);

    var graphemes = new StringInfo(target??"");
    log.Assert(graphemes.LengthInTextElements > 0);
    log.Precondition(0 <= guess && guess <= graphemes.LengthInTextElements);

    log.ExceptionAndThrowIf(
        graphemes.SubstringByTextElements(guess,1) is "💥",
        new ApplicationException("bang!"));

    var remainder = Remove(graphemes,guess);
   
    log.DebugIf(remainder.Length == 0,"TBC:is this permitted?");
    log.If(graphemes.LengthInTextElements > 0,
              helpfulInformation: (target,guess,graphemes.LengthInTextElements),
              label: "Remaining after Removal");

    log.PreconditionNotNull(target);
    log.Postcondition(remainder.Length < target.Length);
    log.Member(remainder);
    return remainder;
}
```
## What is Logged?
 
- All methods log the current Method or Member name.
- All methods can log additional information, either auto-labelled or explicitly labelled.
- Assertions, Pre-, and Post-Conditions log nothing at all if they pass.
- Assertions, Pre-, and Post-Conditions log the literal failed expression if they fail.
- log.If() logs nothing at all if the condition is false
- log.If() logs the literal condition expression if it is true

## Example output

Depending on your logger template configuration:
```
GoBang((target,guess):(12🍾4💥, 1))
GoBang:Precondition Not Null Failed:target:
GoBang:Assertion Failed:graphemes.LengthInTextElements > 0:
GoBang:Precondition Failed:0 <= guess && guess <= graphemes.LengthInTextElements
GoBang:graphemes.LengthInTextElements > 0:Remaining after Removal:(12🍾4💥, 1, 5)
GoBang:Postcondition Failed:0 <= guess && guess <= graphemes.LengthInTextElements
GoBang(remainder:1🍾4💥)
```
## All Methods

All methods accept an optional additonal state parameter, and an optional label (for the state)

```
log.Member()
log.Assert()
log.Precondition()
log.Postcondition()

log.If()
log.DebugIf()
log.VerboseIf()
log.WarningIf()
log.ErrorIf()
log.ExceptionIf()

log.IfNot()
log.DebugIfNot()
log.VerboseIfNot()
log.WarnIfNot()
log.ErrorIfNot()
log.FatalIfNot()

log.Exception()
log.ExceptionAndThrow()
log.FatalAndExitProcessWithExitCode()
```

## Logging additional state

```
//log multiple items of additional state in a ValueTuple or anonymous object.
log.Member( (this,that,other) );

// By default the log line will auto-label the state:
"MemberName:(this,that,other):(value1,value2,value3)"

// You can explicitly label the state:
log.Member( (this,that,other), "Checkpoints 1 to 3")
"MemberName:Checkpoints 1 to 3:(value1,value2,value3)"

// additional state can just be a comment:
log.Member("Comment ...")
```
