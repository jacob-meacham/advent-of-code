defmodule Advent.Day do
  defmacro __using__(no: day_no) when is_integer(day_no) do
    formatted_day_no = "#{day_no}"

    quote do
      @doc """
      Read an input file with the specified file name.
      This should be located in a `input` folder alongside the module.
      """
      def input(filename \\ "day#{unquote(formatted_day_no)}") do
        input_folder = __ENV__.file |> Path.dirname()

        "#{input_folder}/input/#{filename}.txt"
        |> File.read!()
        |> String.trim_trailing()
      end

      def benchmarks do
        %{
          "Day #{unquote(formatted_day_no)}, Part 1" => maybe_run(:run_part1),
          "Day #{unquote(formatted_day_no)}, Part 2" => maybe_run(:run_part2)
        }
        |> Enum.filter(fn {_, fun} -> fun end)
        |> Enum.into(%{})
      end

      # Only run a benchmark if it is defined.
      # It will only be defined if the puzzle has been solved!
      def maybe_run(func_name) do
        if Kernel.function_exported?(__MODULE__, func_name, 0) do
          fn -> Kernel.apply(__MODULE__, func_name, []) end
        end
      end

    end
  end
end
