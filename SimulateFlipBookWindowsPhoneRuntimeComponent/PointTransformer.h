#pragma once

namespace SimulateFlipBookWindowsPhoneRuntimeComponent
{

	public ref class PointTransformer sealed
	{
	public:
		PointTransformer(double a, double b, double c);
		double CalculateTheta(double iX, double iY);
		double PointTransformer::TransformPointX(double iX, double iY, double theta);
		double PointTransformer::TransformPointY(double iX, double iY, double theta);
		double DistanceToAxis(double iX, double iY);
	};
}