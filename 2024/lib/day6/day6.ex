defmodule GuardPatrol do
  @moduledoc "Day 6"

  defp parse_grid(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.with_index
    |> Enum.flat_map(fn {line, row} ->
      line
      |> String.graphemes
      |> Enum.with_index
      |> Enum.map(fn {char, col} -> {{row, col}, char} end)
    end)
    |> Map.new
  end

  defp parse_input(input) do
    rows = String.split(input, "\n", trim: true)
    size = length(rows)

    grid = parse_grid(input)
    walls =
      grid
    |> Enum.filter(fn {_, val} -> val == "#" end)
    |> Enum.map(&elem(&1, 0))
    |> MapSet.new

    guard =
      grid
    |> Enum.find(fn {_, val} -> val == "^" end)
    |> elem(0)

    {size, walls, guard}
  end

  defp simulate(size, walls, direction, guard, seen) do
    if guard_exited?(size, guard) do
      seen
    else
      seen = MapSet.put(seen, guard)
      {new_pos, new_direction} = step(walls, direction, guard)
      simulate(size, walls, new_direction, new_pos, seen)
    end
  end

  # We can check a loop by seeing if the guard ever returns to the exact position and direction
  defp check_loop(size, walls, direction, guard, seen) do
    if guard_exited?(size, guard) do
      false
    else
      if MapSet.member?(seen, {guard, direction}) do
        true
      else
        seen = MapSet.put(seen, {guard, direction})
        {new_pos, new_direction} = step(walls, direction, guard)
        check_loop(size, walls, new_direction, new_pos, seen)
      end
    end
  end

  defp step(walls, {dx, dy}, {gx,  gy}) do
    if MapSet.member?(walls, {gx + dx, gy + dy}) do
      {{gx, gy}, rotate(dx, dy)}
    else
      {{gx + dx, gy + dy}, {dx, dy}}
    end
  end

  defp rotate(-1, 0), do: {0, 1}
  defp rotate(0, 1), do: {1, 0}
  defp rotate(1, 0), do: {0, -1}
  defp rotate(0, -1), do: {-1, 0}

  defp guard_exited?(size, {gx, gy}) do
    gx < 0 or gy < 0 or gx >= size or gy >= size
  end

  def part1(input) do
    parse_input(input)
    |> then(fn {size, walls, guard} ->
      simulate(size, walls, {-1, 0}, guard, MapSet.new())
    end)
    |> MapSet.size
  end

  def part2(input) do
    {size, walls, guard} = parse_input(input)
    :persistent_term.put(GuardPatrol, {walls, guard})

    simulate(size, walls, {-1, 0}, guard, MapSet.new())
    |> MapSet.delete(guard)
    |> Task.async_stream(fn visited_loc ->
      :persistent_term.get(GuardPatrol, {walls, guard})
      check_loop(size, MapSet.put(walls, visited_loc), {-1, 0}, guard, MapSet.new())
    end)
    |> Enum.count(fn {:ok, val} -> val end)
  end
end

{:ok, contents} = File.read("lib/day6/input.txt")
IO.puts(GuardPatrol.part1(contents))
IO.puts(GuardPatrol.part2(contents))

# Benchee.run(%{
#   day6_part1: fn -> contents |> GuardPatrol.part1() end,
#   day6_part2: fn -> contents |> GuardPatrol.part2() end,
#   day6_total: fn ->
#     GuardPatrol.part1(contents)
#     GuardPatrol.part2(contents)
#   end}, warmup: 2,
# time: 3)
