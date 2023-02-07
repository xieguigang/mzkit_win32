imports "RedisApp" from "MZWorkRedis";

src = "";
app = 15963;

keys = RedisApp::load.mzpack(src, app);

print("get ms object redis keys:");
print(keys);

scan1 = RedisApp::scan1(keys[1], app);
keys2 = [scan1]::products;

str(as.list(scan1));

print("get ms2 products:");
str(keys2);

for(id in keys2) {
    scan2 = RedisApp::scan2(id, app);

    print("view of the ms2 spectrum:");
    str(as.list(scan2));
}