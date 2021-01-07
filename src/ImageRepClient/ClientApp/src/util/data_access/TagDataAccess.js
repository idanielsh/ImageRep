import axios from 'axios'

class TagDataAccess {
    constructor() {
        this.tag_axios = axios.create({
            baseURL: 'https://localhost:5401/api/Tag',
        });
    }


    async deleteTag(guid) {
        await this.tag_axios.delete(guid, {})
            .catch(function (error) {
                console.log(error);
                return false;
            });
        return true;
    }

    async addTag(tag) {
        await this.tag_axios.post('', tag)
            .catch(function (error) {
                console.log(error);
                return false;
            });
        return true;
    }

    async getDistinctTags() {
        return await this.tag_axios.get("", {}).then(res => res.data);
    }
}

export default TagDataAccess;