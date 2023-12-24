using Utilities;
using Vec3 = Utilities.VectorUtilities.Vec3;
using Microsoft.Z3;

IEnumerable<(Vec3 pos, Vec3 vel)> GetRays(IEnumerable<string> lines)
{
    var rays = new List<(Vec3 pos, Vec3 vel)>();
    foreach (var l in lines)
    {
        var (posStr, velStr) = l.Split(" @ ");
        var pos = posStr.Split(", ").Select(long.Parse).ToList();
        var vel = velStr.Split(", ").Select(long.Parse).ToList();

        rays.Add((new Vec3(pos[0], pos[1], pos[2]), new Vec3(vel[0], vel[1], vel[2])));
    }

    return rays;
}

long Part1(IEnumerable<string> lines)
{
    var rays = GetRays(lines);
    var count = rays.AllPairs().Count(tuple =>
    {
        var (p1, v1) = tuple.Item1;
        var (p2, v2) = tuple.Item2;
        
        // Need to find points such that
        // p = p1 + v1 * t1
        // p = p2 + v2 * t2
        // has a solution (and whose solution is within the bounds

        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        float det = v2.X * v1.Y - v2.Y * v1.X;
        if (det == 0)
        {
            // Handle parallel
            return false;
        }
        var t1 = (dy * v2.X - dx * v2.Y) / det;
        var t2 = (dy * v1.X - dx * v1.Y) / det;

        if (t1 < 0 || t2 < 0)
        {
            return false;
        }

        var px = p1.X + v1.X * t1;
        var py = p1.Y + v1.Y * t1;
        return px is >= 200000000000000 and <= 400000000000000 &&
               py is >= 200000000000000 and <= 400000000000000;
    });
    
    return count;
}

long Part2(IReadOnlyList<string> lines)
{
    // TODO: Get Z3 working in C# if possible
    // var rays = GetRays(lines).ToList();
    //
    // var ctx = new Context();
    //
    // var x = ctx.MkRealConst("x");
    // var y = ctx.MkRealConst("y");
    // var z = ctx.MkRealConst("z");
    // var vx = ctx.MkRealConst("vx");
    // var vy = ctx.MkRealConst("vy");
    // var vz = ctx.MkRealConst("vz");
    //
    // var solver = ctx.MkSolver();
    //
    // for (var i = 0; i < rays.Count; i++)
    // {
    //     var (pos, vel) = rays[i];
    //     var (zPosX, zPosY, zPosZ) = (ctx.MkReal(pos.X), ctx.MkReal(pos.Y), ctx.MkReal(pos.Z));
    //     var (zVelX, zVelY, zVelZ) = (ctx.MkReal(vel.X), ctx.MkReal(vel.Y), ctx.MkReal(vel.Z));
    //
    //     var t = ctx.MkRealConst($"t_{i}");
    //     solver.Assert(ctx.MkGe(t, ctx.MkReal(0)));
    //     solver.Assert(ctx.MkEq(ctx.MkAdd(x, ctx.MkMul(vx, t)), ctx.MkAdd(zPosX, ctx.MkMul(zVelX, t))));
    //     solver.Assert(ctx.MkEq(ctx.MkAdd(y, ctx.MkMul(vy, t)), ctx.MkAdd(zPosY, ctx.MkMul(zVelY, t))));
    //     solver.Assert(ctx.MkEq(ctx.MkAdd(z, ctx.MkMul(vz, t)), ctx.MkAdd(zPosZ, ctx.MkMul(zVelZ, t))));
    // }
    //
    // if (solver.Check() == Status.SATISFIABLE)
    // {
    //     var m = solver.Model;
    //     var xOut = m.Evaluate(x);
    //     var yOut = m.Evaluate(y);
    //     var zOut = m.Evaluate(z);
    //
    //     Console.WriteLine(xOut);
    //     Console.WriteLine(yOut);
    //     Console.WriteLine(zOut);
    // }


    return 0;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

// Runner.Benchmark(delegate
// {
//     Part1(lines);
//     Part2(lines);
// }, "Day 24");