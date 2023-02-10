imports "RedisApp" from "MZWorkRedis";

options(strict = FALSE);

src = "F:\Temp\mzkit_win32\.cache\9a\9a83a5a2689368d2d268185ab286ac5b.mzPack";
app = 15963;

keys = RedisApp::load.mzpack(src, app);

print("get ms object redis keys:");
print(keys);

scan1_data = RedisApp::scan1(keys[1], app);
keys2 = [scan1_data]::products;

str(as.list(scan1_data));

print("get ms2 products:");
str(keys2);

for(id in keys2) {
    scan2_data = RedisApp::scan2(id, app);

    print("view of the ms2 spectrum:");
    str(as.list(scan2_data));
}