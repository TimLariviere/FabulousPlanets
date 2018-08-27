namespace ElmishPlanets

module Models =
    [<Measure>] type km
    [<Measure>] type s
    [<Measure>] type celsius
    [<Measure>] type Septillion
    [<Measure>] type kg

    type SolarObject =
        {
            Name: string
            Diameter: float<km>
            Temperature: float<celsius>
            Speed: float<km/s>
            Mass: float<Septillion * kg>
            YearOfDiscovery: int option
            Description: string
        }

    type Moon =
        {
            Info: SolarObject
        }

    type Planet =
        {
            Info: SolarObject
            Moons: Moon array
        }

    // Infos from https://www.tes.com/teaching-resource/solar-system-top-trumps-6166065 and http://planetfacts.org
    let solarObjects = [|
        {
            Info = 
                {
                    Name = "Mercury"
                    Diameter = 4878.<km>
                    Temperature = 427.<celsius>
                    Speed = 47.87<km/s>
                    Mass = 0.33<Septillion * kg>
                    YearOfDiscovery = Some 1885
                    Description = "Mercury is the closest planet to th Sun, orbiting our star at an average distance of 57.9 million kilometres, taking 88 days to complete a trip around the sun. Mercury is also the smallest planet in our Solar System."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Venus"
                    Diameter = 12104.<km>
                    Temperature = 482.<celsius>
                    Speed = 35.02<km/s>
                    Mass = 4.86<Septillion * kg>
                    YearOfDiscovery = None
                    Description = "Venus is our neighbouring planet. It is impossible to state when Venus was discovered as it is visible with the naked eye."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Earth"
                    Diameter = 12756.<km>
                    Temperature = 22.<celsius>
                    Speed = 29.78<km/s>
                    Mass = 5.97<Septillion * kg>
                    YearOfDiscovery = None
                    Description = "Our home, some 4.5 billion years old. Life appeared on the surface just 1 billion years after creation with human beings appearing just 200,000 years ago"
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Mars"
                    Diameter = 6794.<km>
                    Temperature = -15.<celsius>
                    Speed = 24.077<km/s>
                    Mass = 0.64<Septillion * kg>
                    YearOfDiscovery = Some 1580
                    Description = "The fourth planet from the Sun. The surface of Mars consists of iron oxide which gives the planet a red appearance, Mars is approximately half the width of the Earth and is also a neighbouring planet to us."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Jupiter"
                    Diameter = 142800.<km>
                    Temperature = -150.<celsius>
                    Speed = 13.07<km/s>
                    Mass = 1898.<Septillion * kg>
                    YearOfDiscovery = Some 1610
                    Description = "Jupiter is the largest planet in our Solar System. Described as a 'gas giant' it orbits our Sun at a distance of 778,000,000 kilometers. A distinct feature of Jupiter is the 'great red spot' which is a storm that has lasted for more the 400 years."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Saturn"
                    Diameter = 120536.<km>
                    Temperature = -180.<celsius>
                    Speed = 9.69<km/s>
                    Mass = 568.<Septillion * kg>
                    YearOfDiscovery = Some -568
                    Description = "Saturn is the sixth planet from the Sun and the second largest planet in our Solar System. Probably best known for the rings that surround it, experts believe these rings formed from a destroyed moon millions of years ago."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Uranus"
                    Diameter = 51118.<km>
                    Temperature = -214.<celsius>
                    Speed = 6.81<km/s>
                    Mass = 86.81<Septillion * kg>
                    YearOfDiscovery = Some 1781
                    Description = "Uranus was the first planet to be discovered by a telescope, and is also visible to the naked eye. Uranus is sometimes referred to as an ice giant."
                }
            Moons = [||]
        }
        {
            Info = 
                {
                    Name = "Neptune"
                    Diameter = 50538.<km>
                    Temperature = -220.<celsius>
                    Speed = 5.43<km/s>
                    Mass = 102.43<Septillion * kg>
                    YearOfDiscovery = Some 1846
                    Description = "Neptune is the eighth planet from the Sun and is 17 times the mass of the Earth. Storms on the planet have wind speeds of up to 2,100 kilometres per hour."
                }
            Moons = [||]
        }
    |]

