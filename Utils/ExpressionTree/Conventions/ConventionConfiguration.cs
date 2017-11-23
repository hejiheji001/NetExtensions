using System.Collections.Generic;
using System.Linq;
using Utils.ExpressionTree.Conventions.Combines;
using Utils.ExpressionTree.Conventions.Properties;

namespace Utils.ExpressionTree.Conventions {
    public class ConventionConfiguration {

        public static ConventionConfiguration Default = new ConventionConfiguration();

        static ConventionConfiguration() {
            Default.Conventions.Add(new ValueTypeEqualsConvention());
            Default.Conventions.Add(new StringContainsConvention());
			Default.Conventions.Add(new InArrayConvention());
			Default.Conventions.Add(new DateEqualsConvention());
            Default.Conventions.Add(new BetweenDatesConvention());
            Default.Conventions.Add(new BetweenValuesConventions());

            //
            Default.Conventions.Add(new AwalysTrueCombineConvention());
            Default.Conventions.Add(new AndCombineConvention());
			Default.Conventions.Add(new OrCombineConvention());
		}

        public ConventionConfiguration() {
            Conventions = new HashSet<IConvention>();
        }
        public HashSet<IConvention> Conventions { get; private set; }

        internal IEnumerable<T> GetConventions<T>() where T : IConvention {
            return Conventions
                .OfType<T>()
                .OrderByDescending(c => c.Order);
        }
    }
}
