package com.example.letsbook.ModalDao;

public class TrainItem {
    private String id;
    private String toStation;
    private String fromStation;
    private String trainType;
    private String created;
    private String trainName;

    public TrainItem(String id, String toStation, String fromStation, String trainType, String created,String trainName) {
        this.id = id;
        this.toStation = toStation;
        this.fromStation = fromStation;
        this.trainType = trainType;
        this.created = created;
        this.trainName = trainName;
    }

    public String getTrainName() {
        return trainName;
    }

    public void setTrainName(String trainName) {
        this.trainName = trainName;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getToStation() {
        return toStation;
    }

    public void setToStation(String toStation) {
        this.toStation = toStation;
    }

    public String getFromStation() {
        return fromStation;
    }

    public void setFromStation(String fromStation) {
        this.fromStation = fromStation;
    }

    public String getTrainType() {
        return trainType;
    }

    public void setTrainType(String trainType) {
        this.trainType = trainType;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }
}
