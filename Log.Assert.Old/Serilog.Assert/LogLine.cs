namespace Serilog.Assert;

public static class LogLine
{
    public const string AssertionFailed = "{Action}:Assertion Failed:{Assertion}:{Label}{State}";
    public const string AssertionNotNullFailed = "{Action}:Assertion Not Null Failed:{Subject}:{Label}{State}";
    public const string AssertionNotNullOrEmptyFailed = "{Action}:Assertion Not Null Or Empty Failed:{Subject}:{Label}{State}";
    public const string AssertionNotNullOrWhitespaceFailed = "{Action}:Assertion Not Null Or Whitespace Failed:{Subject}:{Label}{State}";

    public const string PreconditionFailed = "{Action}:Precondition Failed:{Assertion}:{Label}{State}";
    public const string PreconditionNotNullFailed = "{Action}:Precondition Not Null Failed:{Subject}:{Label}{State}";
    public const string PreconditionNotNullOrEmptyFailed = "{Action}:Precondition Not Null Or Empty Failed:{Subject}:{Label}{State}";
    public const string PreconditionNotNullOrWhitespaceFailed = "{Action}:Precondition Not Null Or Whitespace Failed:{Subject}:{Label}{State}";

    public const string PostconditionFailed = "{Action}:Postcondition Failed:{Assertion}:{Label}{State}";
    public const string PostconditionNotNullFailed = "{Action}:Postcondition Not Null Failed:{Subject}:{Label}{State}";
    public const string PostconditionNotNullOrEmptyFailed = "{Action}:Postcondition Not Null Or Empty Failed:{Subject}:{Label}{State}";
    public const string PostconditionNotNullOrWhitespaceFailed = "{Action}:Postcondition Not Null Or Whitespace Failed:{Subject}:{Label}{State}";

    public const string Member = "{Action}({Label}{@State})";
    public const string MemberWithLine = "{Action}:{Line}({Label}{@State})";
    public const string MemberException = "{Action}({Label}{State})";

    public const string Conditional = "{Action}:{Condition}:{Label}{State}";
    public const string ConditionalWithLine = "{Action}:{Line}:{Condition}:{Label}{State}";

    public const string ExceptionConditional = "{Action}:Condition Failed:{Condition}:{Label}{State}";
}
