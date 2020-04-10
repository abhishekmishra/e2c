#!/usr/bin/env dotnet-script
#r "nuget: MathNet.Numerics, 4.9.0"

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

/*
* The space to clean is a 2D space at the moment where there are three kinds of areas
* i. Empty space (Multiples of negative even numbers), -x/2 indicates number of days this was clean
* ii. Dirty space (Multiples of positive even numbers), x/2 indicates number of days this was dirty
* iii. Wall indicated by 1
*
* The space of integers is kept free for new additions to information of the space.
*/
public static double NODATA = 0;
public static double WALL = 1;

Matrix<Double> space = Matrix<Double>.Build.Dense(4, 4);
space.Clear();

Console.WriteLine("Created new space to clean -> ");
Console.WriteLine(space.ToString());
