namespace ElmishPlanets

module Models =
    [<Measure>] type km
    [<Measure>] type h
    [<Measure>] type celsius
    [<Measure>] type billions
    [<Measure>] type kg

    type SolarObject =
        {
            Name: string
            Diameter: float<km>
            Temperature: float<celsius>
            Speed: float<km/h>
            Mass: float<billions * kg>
            YearOfDiscovery: int
            Description: string
        }

    let solarObjects = [|
        {
            Name = "Mercury"
            Diameter = 4878.<km>
            Temperature = 427.<celsius>
            Speed = 107132.<km/h>
            Mass = 0.33<billions * kg>
            YearOfDiscovery = 1885
            Description = "Mercury is the closest planet to th Sun, orbiting our star at an average distance of 57.9 million kilometres, taking 88 days to complete a trip around the sun. Mercury is also the smallest planet in our Solar System"
        }   
    |]

