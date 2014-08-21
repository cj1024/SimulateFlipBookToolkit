#include "pch.h"
#include "PointTransformer.h"
#include <math.h>

using namespace SimulateFlipBookWindowsPhoneRuntimeComponent;
using namespace Platform;

double _a, _b, _c;

PointTransformer::PointTransformer(double a, double b, double c)
{
	_a = a;
	_b = b;
	_c = c;
}

double PointTransformer::CalculateTheta(double iX, double iY)
{
	return (iX*_a + iY*_b + _c) / (_a*_a + _b*_b);
}

double PointTransformer::TransformPointX(double iX,double iY, double theta)
{
	return iX - 2 * _a*theta;
}

double PointTransformer::TransformPointY(double iX, double iY, double theta)
{
	return iY - 2 * _b*theta;
}

double PointTransformer::DistanceToAxis(double iX, double iY)
{
	return (_a*iX + _b*iY + _c) / sqrt(_a*_a + _b*_b);
}