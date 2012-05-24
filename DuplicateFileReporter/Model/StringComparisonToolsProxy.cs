using System.Collections.Generic;
using System.Linq;
using PureMVC.Patterns;
using SimMetricsApi;
using SimMetricsMetricUtilities;

namespace DuplicateFileReporter.Model
{
	public class StringComparisonToolsProxy : Proxy
	{
		private readonly ICollection<AbstractStringMetric> _metricTools;

		public StringComparisonToolsProxy() : base(Globals.StringComparisonToolsProxy)
		{
			_metricTools = new List<AbstractStringMetric>
			               	{
			               		//new BlockDistance(),
			               		//new ChapmanLengthDeviation(),
			               		//new ChapmanMeanLength(),
			               		//new CosineSimilarity(),
			               		//new DiceSimilarity(),
			               		//new EuclideanDistance(),
			               		//new JaccardSimilarity(),
			               		//new Jaro(),
			               		new JaroWinkler(),
			               		new Levenstein(),
			               		//new MatchingCoefficient(),
			               		new MongeElkan(),
			               		new NeedlemanWunch(),
			               		//new OverlapCoefficient(),
			               		new QGramsDistance(),
			               		//new SmithWaterman(),
			               		new SmithWatermanGotoh(),
			               		//new SmithWatermanGotohWindowedAffine()
			               	};
		}

		public double CompareSimilarities(string a, string b)
		{
			return _metricTools.AsParallel<AbstractStringMetric>().Average(c => c.GetSimilarity(a, b));
		}
	}
}
