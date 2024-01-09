require(graphics2D);

for(name in ["jet" "rainbow" "typhoon" "magma" "plasma" "mako" "rocket" "turbo"]) {
    bitmap(file = `${@dir}/cm_${name}.png`) {
        plot(colorMap.legend(name, mapLevels = 20));
    }
}