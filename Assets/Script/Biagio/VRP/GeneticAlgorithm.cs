using UnityEngine;
using System.Collections;

public class GeneticAlgorithm {

	private int maxIteration;

	public int[][] _population;
	private float[]	_fitness;

	public int[]	_bestSolution;
	public float	_bestFitness = -1f;

	private FitnessFunction fitnessFunction;

	public GeneticAlgorithm(){
		}
	public GeneticAlgorithm(int num_cromosome, int dim_population, int maxIt, FitnessFunction ff) {
		maxIteration = maxIt;

		_fitness = new float[dim_population];
		_population = new int[dim_population][];
		for (int i=0; i<dim_population; i++) {
			_population[i] = new int[num_cromosome];
		}

		fitnessFunction = ff;
		_bestSolution = new int[num_cromosome];

	}

	public void oneStepRun() {
		updateFitness(_population, _fitness, ref _bestFitness, _bestSolution);
		
		int parent1 = -1, parent2 = -1;
		parentsSelection(_fitness, ref parent1, ref parent2);
		int[] offspring = crossover(_population[parent1], _population[parent2]);
		offspring = mutation(offspring);
		int die = killSelection(_fitness);
		
		_population[die] = offspring;
	}

	public int[] run() {
		int iteration = 0;

		first_generation (_population);

		while (iteration < maxIteration) {
			updateFitness(_population, _fitness, ref _bestFitness, _bestSolution);

			int parent1 = -1, parent2 = -1;
			parentsSelection(_fitness, ref parent1, ref parent2);
			int[] offspring = crossover(_population[parent1], _population[parent2]);
			offspring = mutation(offspring);
			int die = killSelection(_fitness);

			_population[die] = offspring;

			iteration++;
		}

		return _bestSolution;
	}

	//DEBUGGED
	public int killSelection(float[] fitness){
		int size = fitness.Length;

		float[] negativeFitness =  new float[size];
		for (int i=0; i<size; i++) {
			negativeFitness[i] = -fitness[i];
		}

		return rankingSelection (negativeFitness);
	}

	//DEBUGGED
	public void parentsSelection(float[] fitness, ref int parent1, ref int parent2) {

		parent1 = rankingSelection (fitness);
		float fitParent1 = fitness [parent1];
		fitness [parent1] = 0.0f;

		parent2 = rankingSelection (fitness);
		fitness [parent1] = fitParent1;

	}

	//DEBUGGED
	public int rankingSelection (float[] fitness) {
		int size = fitness.Length;
		int[] order = new int[size];
		float s = (float)size;

		order = fillOrder (fitness);

		float[] probability = new float[size];
		for (int i=0; i<size; i++) {
			float rank = (float)(size - order[i]);
			probability[i] = (2*rank)/(s*(s-1));
		}
		//printArray (probability, "probability: ");

		return rouletteWheelSelection (probability);
	}

	//DEBUGGED
	public int[] fillOrder(float[] fitness) {
		int size = fitness.Length;
		int[] order = new int[size];
		float[] weights = new float[size];
		for (int i=0; i<size; i++) {
			weights[i] = fitness[i];
			order[i] = i;
		}

		for (int i=0; i<size-1; i++) {
			float max = weights[i];
			int max_i = -1;
			for (int j=i+1; j<size; j++) {
				if(weights[j] > max) {
					max = weights[j];
					max_i = j;
				}
			}
			if (max_i != -1) {
				float aux = weights[i];
				weights[i] = weights[max_i];
				weights[max_i] = aux;
			}
		}

		for (int i=0; i<size; i++) {
			for (int j=0; j<size; j++) {
				if(fitness[i] == weights[j]) order[i] = j;
			}
		}

		return order;
	}

	//DEBUGGED
	public int rouletteWheelSelection(float[] fitness) {
		int size = fitness.Length;
		float total_fitness = 0.0f;
		for (int i=0; i<size; i++) {
			total_fitness += fitness[i];
		}
		
		float[] probability = new float[size];
		for (int i=0; i<size; i++) {
			probability[i] = fitness[i]/total_fitness;
		}

		float sum = 0.0f;
		float random = Random.Range(0.0f, 1.0f); // value between 0 and 1
		for (int i=0; i<size; i++) {
			sum += probability[i];
			if(random < sum) return i;
		}

		return -1;
	}

	public void first_generation(int[][] population) {
		int size = population.Length;
		int length = population [0].Length;

		// inizialize all individial with the basic solution [0, 1, 2, ...]
		for (int i=0; i<size; i++) {
			for (int j=0; j<length; j++) {
				population[i][j] = j;
			}
		}

		//introduce randomness in the population
		for (int i=0; i<size; i++) {
			int many_times = Random.Range(length, length*3);
			for (int j=0; j<many_times; j++) {
				population[i] = mutation(population[i]);
			}
		}
	}

	//DEBUGGED
	public int[] crossover(int[] parent1, int[] parent2) {
		int length = parent1.Length;
		int[] individual = new int[length];

		for (int j=0; j<length; j++) {
			if(j<length/2) individual[j] = parent1[j];
			else           individual[j] =-1;
		}

		int index = length / 2;
		for (int j=length/2; j<length; j++) {
			bool find = false;
			int num = -1;
			while(!find){
			num = parent2[index%length];
			if(isPresent(individual, num)) index++;
				else 	find = true;
			}

			individual[j] = num;
		}

		return individual;
	}

	public bool isPresent(int[] array, int num) {
		foreach (int n in array) {
			if(n==num) return true;
		}
		return false;
	}

	//DEBUGGED
	public int[] mutation(int[] individual) {
		int length = individual.Length;
		int[] newIndividual = new int[length];
		for (int j=0; j<length; j++) {
			newIndividual[j] = individual[j];
		}

		int num1 = -1, num2 = -1;
		while (num1 == num2) {
			num1 = Random.Range(0, length);
			num2 = Random.Range(0, length);
		}
		int aux = newIndividual [num1];
		newIndividual [num1] = newIndividual [num2];
		newIndividual [num2] = aux;

		return newIndividual;
	}

	public void updateFitness(int[][] population, float[] fitness,
	                           ref float bestFitness,  int[] bestSolution) {
		int size = population.Length;

		// inizialize all individial with the basic solution [0, 1, 2, ...]
		for (int i=0; i<size; i++) {
			fitness[i] = calculateFitness(population[i]);
			if (fitness[i] > bestFitness){
				bestFitness = fitness[i];
				copyArray(_bestSolution, population[i]);
			}
		}
	}

	private void copyArray(int[] to, int[] from) {
		for (int i=0; i<from.Length; i++) {
			to[i] = from[i];
		}
	}

	//DEBUGGED
	public float calculateFitness(int[] individial) {
		return fitnessFunction.calculate (individial);
	}


	
	private void printArray(int[] array, string label) {
		string log = label+" [";
		for (int i=0; i<array.Length; i++) {
			log += array[i]+ ", ";
		}
		Debug.Log (log+" ]");
	}

	
	private void printArray(float[] array, string label) {
		string log = label+" [";
		for (int i=0; i<array.Length; i++) {
			log += array[i]+ ", ";
		}
		Debug.Log (log+" ]");
	}

}
