defmodule Mix.Tasks.Day do
  @moduledoc """
  Run all benchmarks and format for README
  """

  use Mix.Task

  def run([]) do
    Mix.Shell.IO.error("Usage: `mix day <number> <part:optional>`")
  end

  def run([day]) do
    run([day, "0"])
  end

  @impl Mix.Task
  def run([day, part]) do
    {:ok, modules} = :application.get_key(:advent_of_code, :modules)

    module = modules
    |> Enum.find(&find_day_module(&1, day))

    p1 = {"Part 1", :run_part1}
    p2 = {"Part 2", :run_part2}

    case part do
      "0" -> [p1, p2]
      "1" -> [p1]
      "2" -> [p2]
      _ -> Mix.Shell.IO.error("Part must be 0, 1, 2")
    end
    |> Enum.map(fn {text, func_name} ->
      Code.ensure_loaded?(module)
      if Kernel.function_exported?(module, func_name, 0) do
        IO.puts("#{text}: #{Kernel.apply(module, func_name, [])}")
      else
        Mix.Shell.IO.error("#{func_name} is undefined for Day #{day}")
      end
    end)
  end

  defp find_day_module(module_name, day) do
    module_name
    |> Atom.to_string()
    |> String.contains?("Day#{day}.")
  end
end
