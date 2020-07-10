using System.IO;
using TestBase;
#if nunit
using NUnit.Framework;
#endif
#if xunit
using Xunit;
#endif
#if (xunit && nunit)
using TheoryAttribute=Xunit.TheoryAttribute;
#endif

namespace FileSystemTest
{
    public class WhenXXX
    {
        const string PathToExampleDocs = "TestDocuments";
        const string ExampleFile1 = "ExampleFile1.txt";
        
        #if nunit
        [TestCase(ExampleFile1)]
        #endif
        #if xunit
        [Theory]
        [InlineData(ExampleFile1)]
        #endif
        public void WhenXXXGivenYYYInTheFileSystem(string fileName)
        {
            var file= new FileInfo(Path.Combine(PathToExampleDocs,fileName));
            using var stream = file.OpenRead();
        }
        
        #if nunit
        [OneTimeSetUp]
        #endif
        #if xunit
        [Fact]
        #endif
        public void EnsureTestDependencies()
        {
            foreach(var file in new[]{ExampleFile1})
            {
                var expectedFile = Path.Combine(PathToExampleDocs, file);
                File.Exists(expectedFile)
                    .ShouldBeTrue(
                        $"Expected to find TestDependency \n\n\"{ExampleFile1}\"\n\n at "
                        + new FileInfo(expectedFile).FullName + " but didn't. \n"
                        + "Include it in the test project and mark it as as BuildAction=Content, " 
                        + "CopyToOutputDirectory=Copy if Newer.\n" +
                        "Then the project build will copy your directory structure of test " +
                        "documents into the right place for tests to find them at the same" +
                        "relative path."
                    );
            }
        }
        #if xunit
        public WhenXXX()
        {
            EnsureTestDependencies();
        }
        #endif
    }    
} 
