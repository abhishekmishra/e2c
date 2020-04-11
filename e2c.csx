#!/usr/bin/env dotnet-script
#r "nuget: MathNet.Numerics, 4.9.0"

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

/*
* The space to clean is a 2D space at the moment where there are three kinds of areas
* i. Empty space (Multiples of negative even numbers), -x/2 + 1 indicates number of days this was clean
* ii. Dirty space (Multiples of positive even numbers), x/2 indicates number of days this was dirty
* iii. Wall/obstacle indicated by 1
* iv. The cleaners can be assigned any odd positive or negative number other than 1
*
* The space of integers is kept free for new additions to information of the space.
*/
public static double NODATA = 0;
public static double WALL = 1;
public static double DIRTY = 2;


public class Space
{

    private Matrix<Double> space;
    private int rows, columns;
    private double wallP, dirtyP;

    public Space(int r, int c, double wallProb = 0.2, double dirtyProb = 0.3)
    {
        rows = r;
        columns = c;
        wallP = wallProb;
        dirtyP = dirtyProb;
        createEmptySpace(rows, columns);
        randomWall();
        randomDirty();
    }

    public void createEmptySpace(int rows, int columns)
    {
        space = Matrix<Double>.Build.Dense(rows, columns);
        space.Clear();
    }

    public void randomWall()
    {
        Random rnd = new Random();
        for (int i = 0; i < space.RowCount; i++)
        {
            for (int j = 0; j < space.ColumnCount; j++)
            {
                double x = rnd.NextDouble();
                if (x <= wallP)
                {
                    space[i, j] = WALL;
                }
            }
        }
    }

    public void randomDirty()
    {
        Random rnd = new Random();
        for (int i = 0; i < space.RowCount; i++)
        {
            for (int j = 0; j < space.ColumnCount; j++)
            {
                if (space[i, j] != WALL)
                {
                    double x = rnd.NextDouble();
                    if (x <= dirtyP)
                    {
                        space[i, j] = DIRTY;
                    }
                }
            }
        }
    }

    public void printSpace()
    {
        for (int i = 0; i < space.RowCount; i++)
        {
            for (int j = 0; j < space.ColumnCount; j++)
            {
                if (space[i, j] == NODATA)
                {
                    Console.Write("▢");
                }
                if (space[i, j] == WALL)
                {
                    Console.Write("◉");
                }
                if (space[i, j] == DIRTY)
                {
                    Console.Write("▩");
                }
            }
            Console.WriteLine();
        }
    }
}


Console.WriteLine("Created new space to clean -> ");
var sp = new Space(10, 10, 0.1, 0.3);
sp.printSpace();