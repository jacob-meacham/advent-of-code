import z3

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

for i, ray in enumerate(rays):
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

print(f'Part 2: {x + y + z}')
