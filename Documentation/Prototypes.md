# Prototypes

Prototypes are a substitute for GameObject prefabs in unity's ECS.
They support inheritance, prototypes can derive from other prototypes and override components of the same type.

# ECSPrototype

The idea is that this class is a generic abstract "template" class that can be used to implement your own prototypes.
Prototypes have 2 generic type parameters which are the component (base-)type and the construction data.

The component base type is a type _ALL_ components you will be able to add to this prototype need to inherit from.
The construction data is used for passing parameters for creation of a prototype, like their worldspace transformation to be set up by components.

For serialization / managing prototypes, prototypes and their components are scriptable objects.

# ECSPrototypeComponent

Base class for implementing components to be used in specialized implementations of ECSPrototype.