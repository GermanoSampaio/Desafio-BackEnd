db = db.getSiblingDB("admin");

const host = "mongodb-rs:27017";

const config = {
    _id: "rs0",
    members: [
        {
            _id: 0,
            host: host
        }
    ]
};

try {
    const status = rs.status();
    print("Replica Set já foi iniciado:");
    printjson(status);
} catch (e) {
    print("Iniciando Replica Set...");
    const result = rs.initiate(config);
    printjson(result);

    let isReady = false;
    while (!isReady) {
        try {
            const status = rs.status();
            if (status.ok === 1) {
                isReady = true;
                print("Replica Set OK");
            } else {
                print("Aguardando Replica Set...");
                start = new Date().getTime();
                do {
                    now = new Date().getTime();
                } while (now - start < 1000);

            }
        } catch (err) {
            print("Aguardando Replica Set...");
            start = new Date().getTime();
            do {
                now = new Date().getTime();
            } while (now - start < 1000);

        }
    }
}
