# loading the required packages
require(ggplot2)
require(ggmap)

lat <- c(TrafficStopData$latitude)
lon <- c(TrafficStopData$longitude)

dataFrame <- as.data.frame(cbind(lon,lat))
centerLat = mean(dataFrame$lat,na.rm=TRUE)
centerLon = mean(dataFrame$lon,na.rm=TRUE)

stopMap <- get_map(location = c(lon = centerLon, lat = centerLat), zoom = 13,
                        maptype = "satellite", scale = 4)

#if point is not on map, you get
#Removed xxx rows containing missing values (geom_point
ggmap(stopMap) +
  geom_point(data = dataFrame, 
             aes(x = lon, y = lat, fill = "red", alpha = 0.8), 
             size = 5, 
             shape = 21) +
  guides(fill=FALSE, alpha=FALSE, size=FALSE)
