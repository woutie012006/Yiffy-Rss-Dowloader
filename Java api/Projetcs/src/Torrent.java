/**
 * Created by woute on 16-8-2015.
 */
public class Torrent {

    private String url;
    private String quality;
    private Integer seeders;
    private Integer peers;
    private String size;

    public Torrent(String url, String quality, Integer seeders, Integer peers, String size) {
        this.url = url;
        this.quality = quality;
        this.seeders = seeders;
        this.peers = peers;
        this.size = size;
    }

    public String getUrl() {

        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getQuality() {
        return quality;
    }

    public void setQuality(String quality) {
        this.quality = quality;
    }

    public Integer getSeeders() {
        return seeders;
    }

    public void setSeeders(Integer seeders) {
        this.seeders = seeders;
    }

    public Integer getPeers() {
        return peers;
    }

    public void setPeers(Integer peers) {
        this.peers = peers;
    }

    public String getSize() {
        return size;
    }

    public void setSize(String size) {
        this.size = size;
    }
}
