defmodule Mix.Tasks.Bench do
  @moduledoc """
  Run all benchmarks and format for README
  """

  use Mix.Task

  def run(_) do
    {:ok, modules} = :application.get_key(:advent_of_code, :modules)

    benchee = Benchee.init()
    |> Benchee.system()

    benchee = modules
    |> Enum.filter(fn module -> only_day_modules(module) end)
    |> Enum.flat_map(&benchmarks/1)
    |> Enum.reduce(benchee, fn {name, func}, acc ->
      Benchee.benchmark(acc, name, func)
    end)

    stats = benchee
    |> Benchee.collect()
    |> Benchee.statistics()

    table_data = stats.scenarios
    |> Enum.group_by(fn scenario ->
      Regex.run(~r/^Day \d+/, scenario.name) |> List.first()
    end)
    |> Enum.map(fn {day, parts} ->
      part_1 = Enum.find(parts, &String.contains?(&1.name, "Part 1"))
      part_2 = Enum.find(parts, &String.contains?(&1.name, "Part 2"))
      part_1_time = part_1.run_time_data.statistics.average / 1_000_000
      part_2_time = part_2.run_time_data.statistics.average / 1_000_000
      total_time = part_1_time + part_2_time
      good = if total_time < 1_000, do: "✅", else: "❌"

      %{
        day: day,
        part_1: Float.round(part_1_time, 2),
        part_2: Float.round(part_2_time, 2),
        total: Float.round(total_time, 2),
        good: good
      }
    end)

    IO.puts "| Day    | Part 1 (ms) | Part 2 (ms) | Total (ms) | Good? |"
    IO.puts "|--------|-------------|-------------|------------|-------|"

    Enum.each(table_data, fn %{day: day, part_1: part_1, part_2: part_2, total: total, good: good} ->
      IO.puts "| #{day}  | #{part_1}        | #{part_2}        | #{total}       | #{good}    |"
    end)

    IO.puts("|--------|-------------|-------------|------------|------|")
    
    part_1_total = Float.round(Enum.reduce(table_data, 0, fn %{part_1: part_1}, acc -> acc + part_1 end), 2)
    part_2_total = Float.round(Enum.reduce(table_data, 0, fn %{part_2: part_2}, acc -> acc + part_2 end), 2)
    grand_total = Float.round(part_1_total + part_2_total, 2)
    num_days = length(table_data)
    avg_time_per_day = grand_total / num_days
    total_good = if avg_time_per_day <= 100, do: "✅", else: "❌"
    
    IO.puts "| Total  | #{part_1_total}        | #{part_2_total}        | #{grand_total}       | #{total_good}    |"
  end

  defp only_day_modules(module_name) do
    module_name
    |> Atom.to_string()
    |> String.match?(~r/Day\d+\./)
  end

  defp benchmarks(module_name) do
    apply(module_name, :benchmarks, [])
  end
end
