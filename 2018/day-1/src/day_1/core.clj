(ns day-1.core)
(require '[clojure.core.reducers :as r])

(defn getFrequencies [fileName]
  (map #(Integer/parseInt %)
      (clojure.string/split-lines
        (slurp fileName))))

(defn parseFrequencies [frequencies]
  (r/fold + frequencies))

(defn findFrequencyCycle
  ([frequencies] (findFrequencyCycle 0 (set []) frequencies))
  ([cur acc coll]
    (let [newValue (+ cur (first coll))]
      (if (contains? acc newValue)
        newValue
        (recur newValue (conj acc newValue) (rest coll))))))



(defn main []
  (let [frequencies (getFrequencies "frequencies.txt")]
    (str "Final Frequency: " (parseFrequencies frequencies) " - "
    "First Cycle: " (findFrequencyCycle (take 1000000 (cycle frequencies))))))

(main)

(defn -main [& args]
  (print (main)))
