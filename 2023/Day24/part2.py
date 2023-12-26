import z3
import time

def benchmark(fn, iterations):
    start = time.perf_counter()
    for _ in range(0, iterations):
        answer = fn()
    end = time.perf_counter()
    return 1000 * ((end - start) / iterations), answer




def main():
    rays = []
    with (open('input.txt', 'r') as f):
        for l in f.readlines():
            l = l.strip()
            pos_part, vel_part = l.split(' @ ')
            pos = [int(p) for p in pos_part.split(', ')]
            vel = [int(v) for v in vel_part.split(', ')]
            rays.append(((pos[0], pos[1], pos[2]), (vel[0], vel[1], vel[2])))


    x, y, z = z3.BitVec('x', 64), z3.BitVec('y', 64), z3.BitVec('z', 64)
    vx, vy, vz = z3.BitVec('vx', 64), z3.BitVec('vy', 64), z3.BitVec('vz', 64)

    solver = z3.Solver()

    # Same answer for a much smaller number of positions, which could probably be done without a solver
    for i, ray in enumerate(rays[0:3]):
        (px, py, pz), (pvx, pvy, pvz) = ray

        t = z3.BitVec(f't_{i}', 64)
        solver.add(t >= 0)
        solver.add(x + vx * t == px + pvx * t)
        solver.add(y + vy * t == py + pvy * t)
        solver.add(z + vz * t == pz + pvz * t)

    assert solver.check() == z3.sat

    m = solver.model()
    x, y, z = m.eval(x), m.eval(y), m.eval(z)
    x, y, z = x.as_long(), y.as_long(), z.as_long()

    return x + y + z

avg_millis, answer = benchmark(main, 10)
print(f'Part 2 Answer: {answer}')
print(f'  {"✅" if avg_millis < 500 else "❌"} average ms: {avg_millis:.2f}')