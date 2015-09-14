using System;
using QSOrmProject.RepresentationModel;

namespace LeaseAgreement.Representations
{
	public interface IPlacesVMFilter : IRepresentationFilter
	{
		DateTime? RestrictStartDate { get;}
		DateTime? RestrictEndDate { get;}
	}
}

