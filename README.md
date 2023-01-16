# RevitGeometryUtils
A library that adds new geometry funcionalities to the Revit API.

# Methods added to existing classes:
## XYZ
### Point methods:
IsNumericallyEqualTo
RoundCoordinates
ProjectOnGlobalPlane
ProjectOnPlaneByPlaneOriginAndNormal
ProjectOnSamePlaneAsPlanarFace

### Vector methods:
IsAlmostParallelTo

## Line
ProjectOnGlobalPlane
ProjectOnSamePlaneAsPlanarFace
ProjectLineOnPlaneByOriginAndNormal
IsAlmostParallelTo
GetAnglesToGlobalAxes
IsSlightlyOffAxis
ExtendByEndAndValue
IsGeometricallyAlmostEqualTo
ExtendByVector
ExtendLineByPointAndValue
ReconstructWithNewPoint

## Arc
ProjectOnGlobalPlane
ProjectOnSamePlaneAsPlanarFace
ProjectOnPlaneByOriginAndNormal

## Ellipse
ProjectOnGlobalPlane

## Curve
IsBelowLengthTolerance
TranslateByVector
ProjectOnGlobalPlane

## CurveLoop

## Plane
GetGlobalPlaneNormal

## PlanarFace
IsAlmostParallelToGlobalPlane
IsParallelToGlobalPlane

## Solid
GetPlanarFacesAsList
TranslateByVector
GetVectorBetweenOriginAndCentroid
GetSolidOutwardFaceDirections
GetSolidOutwardFaceDirection
IsPointInsideSolid
ScaleByValue
GetZNormalFaces
GetFacesParallelToGlobalPlane


# New classes that are useful with existing Revit API geometry:
## CurveSequence
Adds new functionalities to a curve loop to adjust slightly off axis lines, unconnected vertices and off plane lines.
