# Laboratory works for Theory of Finite Automatas and Formar Languages and Theory of Programming Languages 

This project is awful in terms of optimization and dependency isolation but who cares<br/>
Also code quality is even worser than optimization sometimes<br/>
I just hope you will find everything you are looking for here and your journey to the automatas will end up here<br/>

## Structure

### Contexts
* "Grammar" - this is everything that is directly related to the formal languages. You can find there:
  * Left factoring
  * Left recursion elimination
  * Epsilons elimination
  * LL (1) implementation
  
* "FiniteAutomatas" - this is everything related to the finite automatas. You can find there:
  * DFA
  * NFA
  * Minimization algorithm
  * Regular expressions

### Solution folders
* "UI" - all app endpoints are stored here. Take a look to see a usage example or two
* "Tests" - contains everything related to the tests. This is a good place to take a fast breaf into the project
* "Domain" - this is where all models and algorithms are stored
* "Infrastructure" - contains all dependencies that potentially can cause troubles. You can skip everything that is here 
* "Utils" - have all toolkit for better and faster coding. You can skip everything that is here 

### Architecture 
* The whole project is written using modular monolith design principle
* A couple of things is derived from DDD<br/>
