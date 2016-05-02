using System.Collections.Generic;
using System.Collections.Immutable;

namespace Abp.Application.Features
{
    public static class FeatureFinder
    {
        public static IReadOnlyList<Feature> GetAllFeatures(params FeatureProvider[] featureProviders)
        {
            return new InternalFeatureFinder(featureProviders).GetAllFeatures();
        }

        internal class InternalFeatureFinder : FeatureDefinitionContextBase
        {
            public InternalFeatureFinder(params FeatureProvider[] featureProviders)
            {
                foreach (var provider in featureProviders)
                {
                    provider.SetFeatures(this);
                }

                Features.AddAllFeatures();
            }

            public IReadOnlyList<Feature> GetAllFeatures()
            {
                return Features.Values.ToImmutableList();
            }
        }
    }
}
