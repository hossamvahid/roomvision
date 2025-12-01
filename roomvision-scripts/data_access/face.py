class Face:
    def __init__(self, id: str, person_name: str, embedding: list):
        self.id = id
        self.person_name = person_name
        self.embedding = embedding
    
    def getPersonName(self) -> str:
        return self.person_name
    
    def getEmbedding(self) -> list:
        return self.embedding
    
    def getId(self) -> str:
        return self.id