namespace FabulousPlanets

module Models =
    [<Measure>] type km
    [<Measure>] type h
    [<Measure>] type s
    [<Measure>] type celsius
    [<Measure>] type million
    [<Measure>] type septillion
    [<Measure>] type kg
    [<Measure>] type degrees

    type SolarObject =
        { Name: string
          Diameter: float<km>
          Temperature: float<celsius>
          Speed: float<km/s>
          Mass: float<septillion * kg>
          YearOfDiscovery: int option
          Description: string
          AxialTilt: float<degrees>
          RotationPeriod: float<h>
          Rings: bool option }

    type Moon =
        { Info: SolarObject }

    type Planet =
        { Info: SolarObject
          DistanceFromSun: float<million * km>
          Moons: Moon array }

    let converter unit (value: float<'a>) =
        value |> float |> (fun f -> f.ToString("N0") + unit)

    let converterWith2Decimals unit (value: float<'a>) =
        value |> float |> (fun f -> f.ToString("N2") + unit)
       
    let kmToString (km: float<km>) = converter " km" km
    let celsiusToString (degrees: float<celsius>) = converter "°C" degrees
    let speedToString (speed: float<km/s>) = converterWith2Decimals " km/s" speed
    let massToString (mass: float<septillion * kg>) = converterWith2Decimals " * 10^24 kg" mass
    let intOptionToString defaultValue (value: int option) = match value with None -> defaultValue | Some v -> (string v)

    // Infos from https://www.tes.com/teaching-resource/solar-system-top-trumps-6166065, http://planetfacts.org and https://nssdc.gsfc.nasa.gov/planetary/factsheet/
    let solarObjects = [|
        { Info = 
                { Name = "Mercury"
                  Diameter = 4879.<km>
                  Temperature = 427.<celsius>
                  Speed = 47.87<km/s>
                  Mass = 0.330<septillion * kg>
                  YearOfDiscovery = Some 1885
                  Description = "Mercury is the closest planet to th Sun, orbiting our star at an average distance of 57.9 million kilometres, taking 88 days to complete a trip around the sun. Mercury is also the smallest planet in our Solar System."
                  AxialTilt = 0.03<degrees>
                  RotationPeriod = 1407.6<h>
                  Rings = None }
          DistanceFromSun = 57.9<million * km>
          Moons = [||] }
        
        { Info = 
              { Name = "Venus"
                Diameter = 12104.<km>
                Temperature = 482.<celsius>
                Speed = 35.02<km/s>
                Mass = 4.86<septillion * kg>
                YearOfDiscovery = None
                Description = "Venus is our neighbouring planet. It is impossible to state when Venus was discovered as it is visible with the naked eye."
                AxialTilt = 2.64<degrees>
                RotationPeriod = -5832.5<h>
                Rings = None }
          DistanceFromSun = 108.2<million * km>
          Moons = [||] }
        
        { Info = 
              { Name = "Earth"
                Diameter = 12756.<km>
                Temperature = 22.<celsius>
                Speed = 29.78<km/s>
                Mass = 5.97<septillion * kg>
                YearOfDiscovery = None
                Description = "Our home, some 4.5 billion years old. Life appeared on the surface just 1 billion years after creation with human beings appearing just 200,000 years ago"
                AxialTilt = 23.44<degrees>
                RotationPeriod = 23.9<h>
                Rings = None }
          DistanceFromSun = 149.6<million * km>
          Moons = [||] }
        
        { Info = 
              { Name = "Mars"
                Diameter = 6794.<km>
                Temperature = -15.<celsius>
                Speed = 24.077<km/s>
                Mass = 0.64<septillion * kg>
                YearOfDiscovery = Some 1580
                Description = "The fourth planet from the Sun. The surface of Mars consists of iron oxide which gives the planet a red appearance, Mars is approximately half the width of the Earth and is also a neighbouring planet to us."
                AxialTilt = 25.19<degrees>
                RotationPeriod = 24.6<h>
                Rings = None }
          DistanceFromSun = 227.9<million * km>
          Moons = [||] }
        
        { Info = 
              { Name = "Jupiter"
                Diameter = 142800.<km>
                Temperature = -150.<celsius>
                Speed = 13.07<km/s>
                Mass = 1898.<septillion * kg>
                YearOfDiscovery = Some 1610
                Description = "Jupiter is the largest planet in our Solar System. Described as a 'gas giant' it orbits our Sun at a distance of 778,000,000 kilometers. A distinct feature of Jupiter is the 'great red spot' which is a storm that has lasted for more the 400 years."
                AxialTilt = 3.13<degrees>
                RotationPeriod = 9.9<h>
                Rings = None }
          DistanceFromSun = 778.6<million * km>
          Moons = [||] }
        
        { Info = 
              { Name = "Saturn"
                Diameter = 120536.<km>
                Temperature = -180.<celsius>
                Speed = 9.69<km/s>
                Mass = 568.<septillion * kg>
                YearOfDiscovery = Some -568
                Description = "Saturn is the sixth planet from the Sun and the second largest planet in our Solar System. Probably best known for the rings that surround it, experts believe these rings formed from a destroyed moon millions of years ago."
                AxialTilt = 26.73<degrees>
                RotationPeriod = 10.7<h>
                Rings = Some true }
          DistanceFromSun = 1433.5<million * km>
          Moons = [||] }
        
        { Info = 
                { Name = "Uranus"
                  Diameter = 51118.<km>
                  Temperature = -214.<celsius>
                  Speed = 6.81<km/s>
                  Mass = 86.81<septillion * kg>
                  YearOfDiscovery = Some 1781
                  Description = "Uranus was the first planet to be discovered by a telescope, and is also visible to the naked eye. Uranus is sometimes referred to as an ice giant."
                  AxialTilt = 82.23<degrees>
                  RotationPeriod = -17.2<h>
                  Rings = Some true }
          DistanceFromSun = 2872.5<million * km>
          Moons = [||] }
        
        { Info = 
                { Name = "Neptune"
                  Diameter = 50538.<km>
                  Temperature = -220.<celsius>
                  Speed = 5.43<km/s>
                  Mass = 102.43<septillion * kg>
                  YearOfDiscovery = Some 1846
                  Description = "Neptune is the eighth planet from the Sun and is 17 times the mass of the Earth. Storms on the planet have wind speeds of up to 2,100 kilometres per hour."
                  AxialTilt = 28.32<degrees>
                  RotationPeriod = 16.1<h>
                  Rings = None }
          DistanceFromSun = 4495.1<million * km>
          Moons = [||] }
    |]

