window.indexedDBInterop = {
    getRange: function (dbName, storeName, startId, count) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;

                // Check if the store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    db.close();
                    resolve([]); // Return empty array if store doesn't exist
                    return;
                }

                const transaction = db.transaction(storeName, "readonly");
                const store = transaction.objectStore(storeName);

                // Create a range for the query
                const range = IDBKeyRange.lowerBound(startId);

                const getRangeRequest = store.getAll(range);
                getRangeRequest.onsuccess = function(event) {
                    db.close();
                    const result = event.target.result.slice(0, count); // Ensure it doesn't exceed the count
                    resolve(result);
                };

                getRangeRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to get range: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },
    openDatabaseWithCounter: function (dbName, storeName, counterStoreName) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName, 2); // Increment version to force upgrade

            request.onupgradeneeded = function(event) {
                const db = event.target.result;

                // Create or ensure the main store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: "id" });
                }

                // Create or ensure the counter store exists
                if (!db.objectStoreNames.contains(counterStoreName)) {
                    db.createObjectStore(counterStoreName, { keyPath: "id" });
                }
            };

            request.onsuccess = function(event) {
                const db = event.target.result;
                db.close();
                resolve();
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    // For backward compatibility
    openDatabase: function (dbName, storeName) {
        return this.openDatabaseWithCounter(dbName, storeName, "idCounter");
    },

    saveCounter: function (dbName, counterStoreName, counterValue) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;

                // Ensure the counter store exists
                if (!db.objectStoreNames.contains(counterStoreName)) {
                    db.close();
                    // If store doesn't exist, try reinitializing the DB
                    this.openDatabaseWithCounter(dbName, "attackResults", counterStoreName)
                        .then(() => this.saveCounter(dbName, counterStoreName, counterValue))
                        .then(resolve)
                        .catch(reject);
                    return;
                }

                const transaction = db.transaction(counterStoreName, "readwrite");
                const store = transaction.objectStore(counterStoreName);

                const counterObj = { id: "counter", value: counterValue };
                const saveRequest = store.put(counterObj);

                saveRequest.onsuccess = function() {
                    db.close();
                    resolve();
                };

                saveRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to save counter: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    getCounter: function (dbName, counterStoreName) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;

                // Check if the counter store exists
                if (!db.objectStoreNames.contains(counterStoreName)) {
                    db.close();
                    resolve(0); // Default to 0 if store doesn't exist
                    return;
                }

                const transaction = db.transaction(counterStoreName, "readonly");
                const store = transaction.objectStore(counterStoreName);

                const getRequest = store.get("counter");

                getRequest.onsuccess = function(event) {
                    db.close();
                    const result = event.target.result;
                    resolve(result ? result.value : 0);
                };

                getRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to get counter: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    saveItem: function (dbName, storeName, item) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;
                const transaction = db.transaction(storeName, "readwrite");
                const store = transaction.objectStore(storeName);

                const saveRequest = store.put(item);

                saveRequest.onsuccess = function() {
                    db.close();
                    resolve();
                };

                saveRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to save item: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    deleteItem: function (dbName, storeName, id) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;
                const transaction = db.transaction(storeName, "readwrite");
                const store = transaction.objectStore(storeName);

                const deleteRequest = store.delete(id);

                deleteRequest.onsuccess = function() {
                    db.close();
                    resolve();
                };

                deleteRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to delete item: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    clearStore: function (dbName, storeName) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;
                const transaction = db.transaction(storeName, "readwrite");
                const store = transaction.objectStore(storeName);

                const clearRequest = store.clear();

                clearRequest.onsuccess = function() {
                    db.close();
                    resolve();
                };

                clearRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to clear store: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    getAllItems: function (dbName, storeName) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;

                // Check if the store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    db.close();
                    resolve([]); // Return empty array if store doesn't exist
                    return;
                }

                const transaction = db.transaction(storeName, "readonly");
                const store = transaction.objectStore(storeName);

                const getAllRequest = store.getAll();

                getAllRequest.onsuccess = function(event) {
                    db.close();
                    resolve(event.target.result);
                };

                getAllRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to get all items: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    },

    getItemById: function (dbName, storeName, id) {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function(event) {
                const db = event.target.result;

                // Check if the store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    db.close();
                    resolve(null); // Return null if store doesn't exist
                    return;
                }

                const transaction = db.transaction(storeName, "readonly");
                const store = transaction.objectStore(storeName);

                const getRequest = store.get(id);

                getRequest.onsuccess = function(event) {
                    db.close();
                    resolve(event.target.result);
                };

                getRequest.onerror = function(event) {
                    db.close();
                    reject(new Error("Failed to get item: " + event.target.error));
                };
            };

            request.onerror = function(event) {
                reject(new Error("Failed to open database: " + event.target.error));
            };
        });
    }
};