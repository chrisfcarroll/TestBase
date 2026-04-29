PROMPT.md 2026-04-27
=========================

1. Review the xml documentation of public methods in TooString. If there are errors or omissions, correct them. Do not make it more verbose, keep it terse. If there are missing or incorrect parameter comments, correct them.

commit as type 'docs:...'

2. Make the DebugView output for KeyValuePair<,>, where the Key is Scalarish, more compact, like the C# output is.

commit as type 'change:...'

After committing these 2, let's do:

3. The C# output is not valid C# for some of the ScalarishToShortReflectedString output. Let's change the output to be valid, using either anonymous objects or Tuples to keep it compact. For Complex number output, use new {Real=, Complex=}. For Vectors, output Tuples. For Matrices, output nested Tuples.

commit as type 'change:...'

4. Now turn to TestBase.Tests
4.1) Update the version of NUnit in use to the lastest.
4.2) Add a new class for Assertion Comparisons, which will run like a Test fixture, but it will run the report you did comparing the output of NUnit and TestBase. For each kind of assertion, capture the output for TestBase failure and the NUnit output for failure. Print them in the Test output for manual inspection, but also collate all the outputs into a document save on the file system. The point is to compare output example by example so that I can see which is better for each case.
4.3) Can you add something to the comparison report to call into Claude LLM, or any LLM provider, to review the document output and add factual opinions on which is better in each case. In the cases where NUnit is clearly better, the LLM call should also add a terse statement of what should change in TestBase to make it better.
