# Failure Curve of column with FRP (Fiber Reincorced Polymer)

Think of we have a section with following properties:


Concrete: 30MPa
FRP: 210 GPa
Steel: 210GPa

As you can see, we have three types of material: Concrete, FRP and Steel. In this library each material is defined with it's stress-strain curve. In other words, at the background of calculation, all materials are considered as their stress-strain curve and other data is meaningless to the framework. So the framework can support a very large spectrum of materials.



We should also consider a stress-strain curve for concrete and FRP. 

Some materials are predefined in the library, like ParabolicLinear material for concrete, or ElasticPerfectPlastic for steel and FRP. `ParabolicLinear` means material stress-strain relation is parabolic at first and then it is defined with line, thus linear.

Also `ElasticPerfectPlastic` have a perfect linear elastic and then perfect constant plastic.

so first we need 