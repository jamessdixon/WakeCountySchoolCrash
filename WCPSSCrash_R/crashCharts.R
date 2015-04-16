# loading the required packages
require(ggplot2)
require(ggmap)

lat <- c(GeoCodeCrashSite$V4)
lon <- c(GeoCodeCrashSite$V5)
severity <- c(GeoCodeCrashSite$V3,na.rm=TRUE)

#WOW
#severity <-c(GeoCodeCrashSite$V3)
#severity2 <- unique(severity)
#str(severity2)

#unique(GeoCodeCrashSite$V3)
#highCrashes <- GeoCodeCrashSite[GeoCodeCrashSite$V3 == 'A-Injury (Disabling)',]

dataFrame <- as.data.frame(cbind(lon,lat,severity))
centerLat = mean(dataFrame$lat)
centerLon = mean(dataFrame$lon)

#getting the map
#scale 2-20.  2 is contentant and 20 is building.  10 is city scale
crashMap <- get_map(location = c(lon = centerLon, lat = centerLat), zoom = 12, scale = 4)

# plotting the map with some points on it

ggmap(crashMap) +
                geom_point(data = dataFrame, 
                           aes(x = lon, y = lat, fill = "red", alpha = 0.8), 
                           size = 5, 
                           shape = 21) +
                guides(fill=FALSE, alpha=FALSE, size=FALSE)

ggmap(crashMap) + stat_bin2d(data = dataFrame, 
                              aes(x = lon, y = lat, color = as.factor(severity),
                              fill=as.factor(severity),
                              size = .5, bins=30, alpha  = 1/2, h=0.03) 

ggmap(crashMap) + stat_density2d(data = dataFrame,
                                   aes(x=lon,y=lat,fill=as.factor(severity),alpha=as.factor(severity),
                                   bins=4,
                                   geom="polygon",
                                   h=0.03)





