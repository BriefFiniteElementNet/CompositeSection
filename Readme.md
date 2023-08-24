# Intro
This is a scientific/engineering class library/framework for simulating and analysing thick reinforced concrete columns (also general composite section) under axial load and biaxial bending.
As it is scientific it is suggested to first carefully read the pdf intro file (Todo: add pdf file)

## Disclaimer
Code is tested but incomplete, some features may not work

## Features

Matrerial:
- Parabolic-Linear concrete model
- Perfect elastic-plastic steel
- Null material for voids in section.

It is also capable of adding new material like FRP or other types, just need a little code (for more info see `Materials` folder)

## How to use
First you need to define an instance of `CompositeSection` class. This instance will hold all necessary information about the section, like geometry (polygons) and material (stress-strain) curves.

After define and fill new instance of `CompositeSection` class, then you can analyse for a specific strain profile. Outcome can be either section stiffness matrix or internal forces or failure surface.




# Tools used

## license header
- for managing the license header on all files, the 'License Header Manager' extensions is used:
	https://visualstudiogallery.msdn.microsoft.com/5647a099-77c9-4a49-91c3-94001828e99e

Header template is: CompositeSection.licenseheader in root folder
Tutorial for header manager: https://licensemanager.codeplex.com/wikipage?title=Getting%20started&referringTitle=Home

