#Log.Assert( A better style of application logging ).
```
public string GoBang(string target, int guess)
{
    log.Member( (target,guess) );
    log.PreconditionNotNull(target);

    var graphemes = new StringInfo(target??"");
    log.Assert(graphemes.LengthInTextElements > 0);
    log.Precondition(0 <= guess && guess <= graphemes.LengthInTextElements);

    log.LogExceptionAndThrowIf(
        graphemes.SubstringByTextElements(guess,1) is "💥",
        new ApplicationException("bang!"));

    var remainder = Remove(graphemes,guess);
   
    log.LogDebugIf(remainder.Length == 0,"TBC:is this permitted?");
    log.LogIf(graphemes.LengthInTextElements == 0,
              (target,guess,graphemes), 
              helpfulLabel:"Afer Removal");

    log.PreconditionNotNull(target);
    log.Postcondition(remainder.Length < target.Length);
    return remainder;
}
```
## What is Logged?
 
- **All** methods log the current Method or Member name.
- **All** method optionally log additional information, with auto-labelling or an explicit label.
- Assertions, Pre-, and Post-Conditions log nothing at all if they pass.
- Assertions, Pre-, and Post-Conditions log the failed expression if they fail.
- log.If() logs nothing at all if the condition is false
- log.If() logs the condition expression if it is true

## Example output

Depending on your logger template configuration:
```
[typename]GoBang((target,guess):("target💥string", 1))
[typename]GoBang:Precondition Not Null Failed:target:
[typename]GoBang:Assertion Failed:graphemes.LengthInTextElements > 0:
[typename]GoBang:Precondition Failed:0 <= guess && guess <= graphemes.LengthInTextElements
[typename]GoBang:graphemes.LengthInTextElements > 0:Remaining after Removal:(12🍾4💥, 1, 5)
[typename]GoBang:Precondition Failed:0 <= guess && guess <= graphemes.LengthInTextElements
```
## All Methods

All methods accept an optional additonal state parametet, and an optional label (for the state)

```
log.Member()
log.Assert()
log.Precondition()
log.Postcondition()

log.If()
log.DebugIf()
log.TraceIf()
log.WarningIf()
log.ErrorIf()
log.ExceptionIf()

log.Exception()
log.ExceptionAndThrow()
log.CriticalAndExitProcessWithExitCode()
```