TestBase
========
Aims to get you off to a flying start when unit testing projects with many dependencies by 
1) Reducing the amount of boilerplate mock management code you right
2) Providing a rich extensible set of fluent assertions including
* Most NUnit assertions
* ShouldEqualByValue assertion for all kinds of types and collections
* Stream.ShouldCountain assertions
* Moq ShouldCall assertions


Dependencies in this version
------------
* Mocking relies on Moq
* Fluent assertions rely on NUnit (But still work fine in Microsoft Test Projects)


Less useful when
----------------
You don't need mocks and you already have a fluent assertions library
