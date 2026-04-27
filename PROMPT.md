PROMPT.md 2026-04-27
=========================

1. Review the xml documentation of public methods in TooString. If there are errors or omissions, correct them. Do not make it more verbose, keep it terse. If there are missing or incorrect parameter comments, correct them.

commit as type 'docs:...'

2. Make the DebugView output for KeyValuePair<,>, where the Key is Scalarish, more compact, like the C# output is.

commit as type 'change:...'

After committing these 2, let's do:

3. The C# output is not valid C# for some of the ScalarishToShortReflectedString output. Let's change the output to be valid, using either anonymous objects or Tuples to keep it compact. For Complex number output, use new {Real=, Complex=}. For Vectors, output Tuples. For Matrices, output nested Tuples.

commit as type 'change:...'