using System;
using System.Collections.Generic;
#if !NETCOREAPP1_0 && !NETCOREAPP2_0
using System.Transactions;
#endif
using odec.Framework.Generic;

namespace Exchange.Runner.TestRealization
{
    public class TestFakeObject : Glossary<int>
    {

        public static IEnumerable<TestFakeObject> GenerateFakeObjects(int number)
        {
            var result = new List<TestFakeObject>();
            for (var i = 0; i < number; i++)
            {
                result.Add(new TestFakeObject
                {
                    Code = Guid.NewGuid().ToString(),
                    Id = i,
                    SortOrder = i,
                    IsActive = i % 2 == 0
                });
            }

            return result;

        }
    }
}
